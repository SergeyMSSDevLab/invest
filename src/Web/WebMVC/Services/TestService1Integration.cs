using Microsoft.Extensions.Logging;
using MssDevLab.Common.Http;
using MssDevLab.Common.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    internal class TestService1Integration : BaseHttpCaller, ITestService1Integration
    {
        private readonly ILogger _logger;
        protected override ILogger Log => _logger;

        public TestService1Integration(HttpClient httpClient, ILogger<TestService1Integration> logger) : base(httpClient)
        {
            _logger = logger;
            string baseUrl = "http://mssproto-test-service1";    // TODO: get from settings

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ArgumentException($"{baseUrl} is not a wellformed absolute uri");
            }

            httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            _logger.LogDebug("TestService1Integration instance created, base address: '{baseUrl}'", baseUrl);
        }

        public async Task<ServiceResponse> FetchData(ServiceRequest request)
        {
            try
            {
                var response = await PostAsync<ServiceRequest, ServiceResponse>("TestService1/FetchData", request).ConfigureAwait(false);
                if (response != null && response.IsSuccessCode && response.Result != null)
                {
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from path 'TestService1/FetchData'");
            }

            return new ServiceResponse
            {
                ServiceType = ServiceType.TestService1,
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
