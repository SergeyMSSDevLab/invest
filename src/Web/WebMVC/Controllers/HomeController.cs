using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MssDevLab.Common.Models;
using MssDevLab.WebMVC.Models;
using MssDevLab.WebMVC.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITestServiceIntegration _testService;

        public HomeController(ILogger<HomeController> logger, ITestServiceIntegration testService)
        {
            _logger = logger;
            _testService = testService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _testService.GetAsync<IEnumerable<WeatherForecast>>("WeatherForecast", "homeController");
            _logger.LogInformation($"HomeController gets items from test service status code:{items.StatusCode}, items:{items.Result.Count()}", DateTime.UtcNow);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}