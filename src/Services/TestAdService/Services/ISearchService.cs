using MssDevLab.Common.Models;

namespace MssDevLab.TestAdService.Services
{
    public interface ISearchService
    {
        Task<SearchCompletedEvent> FetchAdsAsync(SearchRequestedEvent requestData);
    }
}