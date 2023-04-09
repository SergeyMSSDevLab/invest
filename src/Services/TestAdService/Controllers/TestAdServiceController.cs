using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;

namespace MssDevLab.TestAdService.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}