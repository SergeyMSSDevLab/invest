﻿using Microsoft.Extensions.Logging;
using MssDevLab.Common.Http;
using MssDevLab.Common.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    internal class TestServiceIntegration : BaseHttpCaller, ITestServiceIntegration
    {
        private readonly ILogger _logger;
        protected override ILogger Log => _logger;

        public TestServiceIntegration(HttpClient httpClient, ILogger<TestServiceIntegration> logger) : base(httpClient)
        {
            _logger = logger;
            string baseUrl = "http://mssproto-test-service";    // TODO: get from settings

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ArgumentException($"{baseUrl} is not a wellformed absolute uri");
            }

            httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            _logger.LogDebug($"TestServiceIntegration instance created, base address: '{baseUrl}'");
        }

        public async Task<ServiceResponse?> FetchData(ServiceRequest request)
        {
            var response = await PostAsync<ServiceRequest, ServiceResponse>("TestService/FetchData", request).ConfigureAwait(false);
            if (response != null && response.IsSuccessCode)
            {
                return response.Result;
            }

            return null;
        }
    }
}
