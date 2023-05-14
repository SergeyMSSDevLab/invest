using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MssDevLab.Common.Extensions;
using MssDevLab.Common.Http;
using MssDevLab.Common.Models;
using System;
using System.Linq;
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
            _logger.LogDebug("TestAdServiceIntegration instance created, base address: '{baseUrl}'", baseUrl);
        }

        public async Task<SearchCompletedEvent> FetchAds(SearchRequestedEvent request)
        {
            try
            {
                var response = await PostAsync<SearchRequestedEvent, SearchCompletedEvent>("TestAdService/FetchAds", request).ConfigureAwait(false);
                if (response != null && response.IsSuccessCode && response.Result != null)
                {
                    return response.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ads from path 'TestAdService/FetchAds'");
            }

            return new SearchCompletedEvent 
                { 
                    ServiceType = ServiceType.AdService,
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
