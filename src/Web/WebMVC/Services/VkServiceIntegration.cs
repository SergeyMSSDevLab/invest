﻿using Microsoft.Extensions.Logging;
using MssDevLab.Common.Http;
using MssDevLab.Common.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    internal class VkServiceIntegration : BaseHttpCaller, IVkServiceIntegration
    {
        private readonly ILogger _logger;
        protected override ILogger Log => _logger;

        public VkServiceIntegration(HttpClient httpClient, ILogger<VkServiceIntegration> logger) : base(httpClient)
        {
            _logger = logger;
            string baseUrl = "http://mssproto-vk-service";    // TODO: get from settings

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ArgumentException($"{baseUrl} is not a wellformed absolute uri");
            }

            httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            _logger.LogDebug("TestServiceIntegration instance created, base address: '{baseUrl}'", baseUrl);
        }

        public async Task<ServiceResponse> FetchData(ServiceRequest request)
        {
            try
            {
                var response = await PostAsync<ServiceRequest, ServiceResponse>("VkService/FetchData", request).ConfigureAwait(false);
                if (response != null && response.IsSuccessCode && response.Result != null)
                {
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from path 'VkService/FetchData'");
            }

            return new ServiceResponse
            {
                ServiceType = ServiceType.VkService,
                QueryString = request.QueryString,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                ItemsAmount = 0,
                Items = Array.Empty<ServiceData>(),
                IsSuccesfull = false
            };
        }
    }
}
