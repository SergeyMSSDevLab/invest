using Microsoft.Extensions.Logging;
using MssDevLab.Common.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    internal class TestServiceIntegration : BaseHttpCaller, ITestServiceIntegration
    {
        private readonly ILogger _logger;

        private const int OneMinuteInMilliseconds = 60000;

        protected override ILogger Log => _logger;

        public TestServiceIntegration(ILogger<TestServiceIntegration> logger)
        {
            _logger = logger;
        }

        protected override Task<HttpClient> CreateHttpClientAsync(TimeSpan? timeOut)
        {
            HttpClient httpClient = CreateHttpClientInternal(timeOut);
            return Task.FromResult(httpClient);
        }


        protected override HttpClient CreateHttpClient(TimeSpan? timeOut)
        {
            HttpClient httpClient = CreateHttpClientInternal(timeOut);
            return httpClient;
        }

        private HttpClient CreateHttpClientInternal(TimeSpan? timeOut)
        {
            HttpClient httpClient  = new HttpClient();

            httpClient.Timeout = timeOut ?? TimeSpan.FromMilliseconds(OneMinuteInMilliseconds);

            string baseUrl = "http://mssproto-test-service";
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
            {
                throw new ArgumentException($"{baseUrl} is not a wellformed absolute uri");
            }
            httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);

            //ServicePointManager.FindServicePoint(httpClient.BaseAddress).ConnectionLeaseTimeout =
            //    ApplicationSettings.ServicePointManager.ConnectionLeaseTimeout ?? OneMinuteInMilliseconds;


            //var byteArray = Encoding.ASCII.GetBytes($"{ApplicationSettings.Boomi.UserName}:{ApplicationSettings.Boomi.Password}");
            //httpClient.DefaultRequestHeaders.Authorization =
            //    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            Log.LogDebug($"HttpClient created, base address: '{baseUrl}'");
            return httpClient;
        }
    }
}
