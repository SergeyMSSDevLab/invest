using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;

namespace MssDevLab.YtService.Services
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
            _logger.LogDebug($"YtService.SearchService.FetchDataAsync called");
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
                PageSize = requestData.PageSize,
                QueryString = requestData.QueryString,
                ServiceType = ServiceType.YtService,
                IsSuccesfull = true,
                ConnectionId = requestData.ConnectionId
            };
            var items = new List<ServiceData>();
            for (int i = 0; i < ret.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = requestData.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.YtService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "http://www.mssdevlab.com/img/zoom.png",
                    Title = $"YtService index:{i + 1} page:{requestData.PageNumber}",
                    Description = $"Example of the data from provider. YtService email:'{email}' query:'{requestData.QueryString}'",
                    Relevance = i
                };
                items.Add(data);
            }
            ret.Items = items.ToArray();

            return await Task.FromResult(ret);
        }
    }
}
