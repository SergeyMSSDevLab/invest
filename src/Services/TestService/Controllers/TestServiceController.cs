using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;

namespace MssDevLab.TestService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestServiceController : ControllerBase
    {
        private readonly ILogger<TestServiceController> _logger;

        public TestServiceController(ILogger<TestServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTestService")]
        public IEnumerable<ServiceData> Get()
        {
            _logger.LogInformation("GetTestService called");
            return Enumerable.Empty<ServiceData>();
        }
    }
}