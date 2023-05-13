using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;

namespace MssDevLab.TestService.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;

        public SearchService(ILogger<SearchService> logger)
        {
            _logger = logger;
        }

        public async Task<SearchCompletedEvent> FetchDataAsync(SearchRequestedEvent requestData)
        {
            _logger.LogDebug($"TestService.SearchService.FetchDataAsync called");
            string? email = null;
            if (requestData.UserPreferences != null)
            {
                // Do something if user authentificated
                email = requestData.UserPreferences.Email;
            }

            // Prepare request to actual API

            // Prepare response
            var ret = new SearchCompletedEvent()
            {
                ItemsAmount = int.MaxValue,    // TODO: Retrieve items amount from underlying service
                PageNumber = requestData.PageNumber,
                PageSize = 3,   // TODO: just for debug
                QueryString = requestData.QueryString,
                ServiceType = ServiceType.TestService,
                IsSuccesfull = true,
                ConnectionId = requestData.ConnectionId
            };
            var items = new List<ServiceData>();
            for (int i = 0; i < ret.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = requestData.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.TestService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "http://www.mssdevlab.com/img/zoom.png",
                    Title = $"TestService index:{i + 1} page:{requestData.PageNumber}",
                    Description = $"Example of the data from provider. TestService email:'{email}' query:'{requestData.QueryString}'",
                    Relevance = i
                };
                items.Add(data);
            }
            ret.Items = items.ToArray();

            return await Task.FromResult(ret);
        }
    }
}
