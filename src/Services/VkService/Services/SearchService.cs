using Microsoft.AspNetCore.Mvc;
using MssDevLab.Common.Models;
using VkNet.Model.RequestParams;
using VkNet.Model;
using VkNet;

namespace MssDevLab.VkService.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly string _defaultAccessToken;

        public SearchService(ILogger<SearchService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _defaultAccessToken = configuration["Secrets:Vk:DefaultAccessToken"] ?? string.Empty;
            _logger.LogDebug("VkService.SearchService._defaultAccessToken: {_defaultAccessToken}", _defaultAccessToken);
        }

        public async Task<SearchCompletedEvent> FetchDataAsync(SearchRequestedEvent requestData)
        {
            _logger.LogDebug($"VkService.SearchService.FetchDataAsync called");
            string? email = null;
            if (requestData.UserPreferences != null)
            {
                // Do something if user authentificated
                email = requestData.UserPreferences.Email;
            }

            var items = new List<ServiceData>();
            if (!string.IsNullOrWhiteSpace(requestData.QueryString) && !string.IsNullOrEmpty(_defaultAccessToken))  // TODO: consider individual token
            {
                // Prepare request to actual API
                var api = new VkApi();

                api.Authorize(new ApiAuthParams
                {
                    AccessToken = _defaultAccessToken
                });

                var offset = 0;
                if (requestData.PageNumber > 1) 
                { 
                    offset = (requestData.PageNumber - 1) * requestData.PageSize;
                }

                var req = new VideoSearchParams()
                {
                    Adult = true,
                    Count = requestData.PageSize,
                    Offset = offset,
                    Sort = VkNet.Enums.VideoSort.Relevance,
                    Query = requestData.QueryString
                };


                VkNet.Utils.VkCollection<VkNet.Model.Attachments.Video> res = await api.Video.SearchAsync(req);
                foreach (var v in res)
                {
                    var data = new ServiceData
                    {
                        Id = v.Id.ToString(),
                        Type = ServiceType.VkService,
                        Url = v.Player.AbsoluteUri,
                        ImageUrl = v.Image?.FirstOrDefault()?.Url.AbsoluteUri,
                        Title = v.Title,
                        Description = v.Description,
                        Relevance = 5
                    };
                    items.Add(data);
                }
            }

            // Prepare response
            var ret = new SearchCompletedEvent()
            {
                ItemsAmount = int.MaxValue,    // TODO: Retrieve items amount from underlying service
                PageNumber = requestData.PageNumber,
                PageSize = requestData.PageSize,
                QueryString = requestData.QueryString,
                ServiceType = ServiceType.VkService,
                IsSuccesfull = true,
                ConnectionId = requestData.ConnectionId,
                Items = items.ToArray()
            };

            return ret;
        }
    }
}
