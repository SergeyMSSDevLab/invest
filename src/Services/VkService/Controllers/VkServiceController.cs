using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;
using Serilog;
using System.Net;

namespace MssDevLab.VkService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VkServiceController : ControllerBase
    {
        private readonly ILogger<VkServiceController> _logger;

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

            // Prepare request to actual API

            // Prepare response
            var ret = new ServiceResponse()
            {
                ItemsAmount = int.MaxValue,    // TODO: Retrieve items amount from underlying service
                PageNumber = requestData.PageNumber,
                PageSize = requestData.PageSize,
                QueryString = requestData.QueryString,
                ServiceType = ServiceType.VkService,
                IsSuccesfull = true
            };
            var items = new List<ServiceData>();
            for (int i = 0; i < requestData.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = requestData.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.VkService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "",
                    Title = $"VkService index:{i + 1} page:{requestData.PageNumber}",
                    Description = $"Example of the data from provider. VkService email:'{email}' query:'{requestData.QueryString}'",
                    Relevance = i
                };
                items.Add(data);
            }
            ret.Items = items.ToArray();

            return Ok(await Task.FromResult(ret));
        }
    }
}