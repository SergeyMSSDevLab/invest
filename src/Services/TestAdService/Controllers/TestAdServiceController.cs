using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;
using MssDevLab.TestAdService.Services;
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
        private readonly ISearchService _searchService;

        public TestAdServiceController(ILogger<TestAdServiceController> logger, ISearchService searchService)
        {
            _logger = logger;
            _searchService = searchService;
        }

        [HttpGet(Name = "GetTestAdService")]
        public IEnumerable<ServiceData> Get()
        {
            _logger.LogInformation("GetTestAdService called");
            return Enumerable.Empty<ServiceData>();
        }

        [HttpPost(Name = "FetchAds")]
        [ProducesResponseType(typeof(SearchCompletedEvent), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<SearchCompletedEvent>> FetchAdsAsync([FromBody] SearchRequestedEvent requestData)
        {
            _logger.LogDebug($"TestAdService.TestAdServiceController.FetchAdsAsync called");
            var ret = await _searchService.FetchAdsAsync(requestData);

            return Ok(await Task.FromResult(ret));
        }
    }
}