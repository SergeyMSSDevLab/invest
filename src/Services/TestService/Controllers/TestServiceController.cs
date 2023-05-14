using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;
using MssDevLab.TestService.Services;
using Serilog;
using System.Net;

namespace MssDevLab.TestService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestServiceController : ControllerBase
    {
        private readonly ILogger<TestServiceController> _logger;
        private readonly ISearchService _searchService;

        public TestServiceController(ILogger<TestServiceController> logger, 
            ISearchService searchService)
        {
            _logger = logger;
            _searchService = searchService;
        }

        [HttpPost(Name = "FetchData")]
        [ProducesResponseType(typeof(SearchCompletedEvent), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<SearchCompletedEvent>> FetchDataAsync([FromBody] SearchRequestedEvent requestData)
        {
            _logger.LogDebug($"TestService.TestServiceController.FetchDataAsync called");
            return Ok(await _searchService.FetchDataAsync(requestData));
        }
    }
}