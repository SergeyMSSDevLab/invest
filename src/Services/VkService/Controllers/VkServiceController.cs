using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;
using Serilog;
using System.Net;
using VkNet.Model.RequestParams;
using VkNet.Model;
using VkNet;

namespace MssDevLab.VkService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VkServiceController : ControllerBase
    {
        private readonly ILogger<VkServiceController> _logger;
        private readonly string AccessToken = "";

        public VkServiceController(ILogger<VkServiceController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "FetchData")]
        [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ServiceResponse>> FetchDataAsync([FromBody] ServiceRequest requestData)
        {
            _logger.LogDebug($"VkService.TestServiceController.FetchDataAsync called");
            string? email = null;
            if (requestData.UserPreferences != null)
            {
                // Do something if user authentificated
                email = requestData.UserPreferences.Email;
            }

            var items = new List<ServiceData>();
            if (!string.IsNullOrWhiteSpace(requestData.QueryString))
            {
                // Prepare request to actual API
                var api = new VkApi();

                api.Authorize(new ApiAuthParams
                {
                    AccessToken = AccessToken
                });

                var req = new VideoSearchParams()
                {
                    Adult = true,
                    Count = requestData.PageSize,
                    Offset = 0,
                    Sort = VkNet.Enums.VideoSort.Relevance,
                    Query = requestData.QueryString
                };


                VkNet.Utils.VkCollection<VkNet.Model.Attachments.Video> res = await api.Video.SearchAsync(req);
                foreach (var v in res)
                {
                    var data = new ServiceData
                    {
                        Id = v.Id.ToString(),
                        Type = ServiceType.VkService,
                        Url = v.Player.AbsoluteUri,
                        ImageUrl = v.Image?.FirstOrDefault()?.Url.AbsoluteUri,
                        Title = v.Title,
                        Description = v.Description,
                        Relevance = 5
                    };
                    items.Add(data);
                }
            }

            // Prepare response
            var ret = new ServiceResponse()
            {
                ItemsAmount = int.MaxValue,    // TODO: Retrieve items amount from underlying service
                PageNumber = requestData.PageNumber,
                PageSize = requestData.PageSize,
                QueryString = requestData.QueryString,
                ServiceType = ServiceType.VkService,
                IsSuccesfull = true,
                Items = items.ToArray()
            };

            return Ok(await Task.FromResult(ret));
        }
    }
}