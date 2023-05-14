using MssDevLab.Common.Models;

namespace MssDevLab.TestAdService.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;

        public SearchService(ILogger<SearchService> logger)
        {
            _logger = logger;
        }

        public async Task<SearchCompletedEvent> FetchAdsAsync(SearchRequestedEvent requestData)
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
                PageSize = requestData.PageSize,
                QueryString = requestData.QueryString,
                ServiceType = ServiceType.AdService,
                IsSuccesfull = true,
                ConnectionId = requestData.ConnectionId
            };
            var items = new List<ServiceData>();
            for (int i = 0; i < ret.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = requestData.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.AdService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "http://www.mssdevlab.com/img/birthdays.png",
                    Title = $"TestAdService index:{i + 1} page:{requestData.PageNumber}",
                    Description = $"Example of the advertisment from provider. TestAdService email:'{email}' query:'{requestData.QueryString}'",
                    Relevance = i
                };
                items.Add(data);
            }
            ret.Items = items.ToArray();

            return await Task.FromResult(ret);
        }
    }
}
