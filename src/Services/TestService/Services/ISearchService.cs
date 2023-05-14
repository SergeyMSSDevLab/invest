using MssDevLab.Common.Models;

namespace MssDevLab.TestService.Services
{
    public interface ISearchService
    {
        Task<SearchCompletedEvent> FetchDataAsync(SearchRequestedEvent requestData);
    }
}