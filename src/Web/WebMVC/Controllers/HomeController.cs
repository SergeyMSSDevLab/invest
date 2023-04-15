using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MssDevLab.Common.Models;
using MssDevLab.WebMVC.Models;
using MssDevLab.WebMVC.Services;
using MssDevLab.WebMVC.ViewModels;
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
        private readonly ITestAdServiceIntegration _testAdService;
        private readonly ITestServiceIntegration _testService;
        private readonly ITestService1Integration _testService1;

        public HomeController(ILogger<HomeController> logger, 
            ITestAdServiceIntegration testAdService, 
            ITestServiceIntegration testService, 
            ITestService1Integration testService1)
        {
            _logger = logger;
            _testAdService = testAdService;
            _testService = testService;
            _testService1 = testService1;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequest = new ServiceRequest
            {
                PageNumber = 1,
                PageSize = 5,
                QueryString = string.Empty,
                UserPreferences = null
            };

            var userName = User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                serviceRequest.UserPreferences = new UserData { Email = userName };
            }

            var adsResponse = await _testAdService.FetchAds(serviceRequest);
            _logger.LogInformation("HomeController gets items from test ad service. Items:{itemsCount}", adsResponse?.Items?.Count());
            var viewModel = new HomeIndexViewModel(adsResponse?.Items);

            var serviceResponse = await _testService.FetchData(serviceRequest);
            _logger.LogInformation("HomeController gets items from test  service. Items:{itemsCount}", serviceResponse?.Items?.Count());

            var service1Response = await _testService1.FetchData(serviceRequest);
            _logger.LogInformation("HomeController gets items from test  service1. Items:{itemsCount}", service1Response?.Items?.Count());

            var dataList = new List<ServiceData>();
            dataList.AddRange(serviceResponse?.Items ?? Enumerable.Empty<ServiceData>());
            dataList.AddRange(service1Response?.Items ?? Enumerable.Empty<ServiceData>());
            viewModel.WallItems = dataList;

            return View(viewModel);
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