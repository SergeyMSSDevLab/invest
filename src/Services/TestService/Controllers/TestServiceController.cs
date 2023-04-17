using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;
using Serilog;
using System.Net;

namespace MssDevLab.TestService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestServiceController : ControllerBase
    {
        private readonly ILogger<TestServiceController> _logger;

        public TestServiceController(ILogger<TestServiceController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "FetchData")]
        [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ServiceResponse>> FetchDataAsync([FromBody] ServiceRequest requestData)
        {
            _logger.LogDebug($"TestService.TestServiceController.FetchDataAsync called");
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
                ServiceType = ServiceType.TestService,
                IsSuccesfull = true
            };
            var items = new List<ServiceData>();
            for (int i = 0; i < requestData.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = requestData.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.TestService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "http://www.mssdevlab.com/img/zoom.png",
                    Title = $"TestService index:{i + 1} page:{requestData.PageNumber}",
                    Description = $"Example of the data from provider. TestService email:'{email}' query:'{requestData.QueryString}'",
                    Relevance = i
                };
                items.Add(data);
            }
            ret.Items = items.ToArray();

            return Ok(await Task.FromResult(ret));
        }
    }
}