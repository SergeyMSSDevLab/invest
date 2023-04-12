﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly ITestAdServiceIntegration _testAdService;

        public HomeController(ILogger<HomeController> logger, ITestAdServiceIntegration testAdService)
        {
            _logger = logger;
            _testAdService = testAdService;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequest = new ServiceRequest
            {
                PageNumber = 1,
                PageSize = 10,
                QueryString = string.Empty,
                UserPreferences = null
            };
            var adsResponse = await _testAdService.FetchAds(serviceRequest);
            _logger.LogInformation("HomeController gets items from test ad service. Items:{itemsCount}", adsResponse?.Items?.Count());
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