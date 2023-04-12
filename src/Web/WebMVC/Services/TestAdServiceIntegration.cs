using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MssDevLab.Common.Extensions;
using MssDevLab.Common.Http;
using MssDevLab.Common.Models;
using System;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{

    public class TestAdServiceIntegration : BaseHttpCaller, ITestAdServiceIntegration
    {
        private readonly ILogger _logger;
        protected override ILogger Log => _logger;

        public TestAdServiceIntegration(HttpClient httpClient, ILogger<TestAdServiceIntegration> logger) : base(httpClient)
        {
            _logger = logger;
            string baseUrl = "http://mssproto-test-ad-service";    // TODO: get from settings

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ArgumentException($"{baseUrl} is not a wellformed absolute uri");
            }

            httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            Log.LogDebug($"TestAdServiceIntegration instance created, base address: '{baseUrl}'");
        }

        public async Task<ServiceResponse?> FetchAds(ServiceRequest request)
        {
            var response = await PostAsync<ServiceRequest, ServiceResponse>("TestAdService/FetchAds", request).ConfigureAwait(false);
            if (response != null && response.IsSuccessCode)
            {
                return response.Result;
            }

            return null;
        }
    }
}
