using MssDevLab.Common.Models;

namespace MssDevLab.YtService.Services
{
    public interface ISearchService
    {
        Task<SearchCompletedEvent> FetchDataAsync(SearchRequestedEvent requestData);
    }
}