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

        private readonly string _defaultAccessToken;

        public VkServiceController(ILogger<VkServiceController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _defaultAccessToken = configuration["Secrets:DefaultVkToken"] ?? string.Empty;
        }

        [HttpPost(Name = "FetchData")]
        [ProducesResponseType(typeof(SearchCompletedEvent), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<SearchCompletedEvent>> FetchDataAsync([FromBody] SearchRequestedEvent requestData)
        {
            _logger.LogDebug($"VkService.TestServiceController.FetchDataAsync called");
            _logger.LogDebug("VkService.TestServiceController._defaultAccessToken: {_defaultAccessToken}", _defaultAccessToken);
            string? email = null;
            if (requestData.UserPreferences != null)
            {
                // Do something if user authentificated
                email = requestData.UserPreferences.Email;
            }

            var items = new List<ServiceData>();
            if (!string.IsNullOrWhiteSpace(requestData.QueryString) && !string.IsNullOrEmpty(_defaultAccessToken))  // TODO: consider individual token
            {
                // Prepare request to actual API
                var api = new VkApi();

                api.Authorize(new ApiAuthParams
                {
                    AccessToken = _defaultAccessToken
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
            var ret = new SearchCompletedEvent()
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