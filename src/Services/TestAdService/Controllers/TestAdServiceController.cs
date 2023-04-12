using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;
using System.Collections;
using System.Net;
using System.Net.Http;

namespace MssDevLab.TestAdService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestAdServiceController : ControllerBase
    {
        private readonly ILogger<TestAdServiceController> _logger;

        public TestAdServiceController(ILogger<TestAdServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTestAdService")]
        public IEnumerable<ServiceData> Get()
        {
            _logger.LogInformation("GetTestAdService called");
            return Enumerable.Empty<ServiceData>();
        }

        [HttpPost(Name = "FetchAds")]
        [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ServiceResponse>> FetchAdsAsync([FromBody] ServiceRequest requestData)
        {
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
                QueryString = requestData.QueryString
            };
            var items = new List<ServiceData>();
            for(int i = 0; i < requestData.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = requestData.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.AdService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "http://www.mssdevlab.com/img/birthdays.png",
                    Title = $"TestAdService index:{i + 1} page:{requestData.PageNumber}",
                    Description = $"Example of the advertisment. TestAdService email:'{email}' query:'{requestData.QueryString}'"

                };
                items.Add(data);
            }
            ret.Items = items;

            return Ok(await Task.FromResult(ret));
        }
    }
}