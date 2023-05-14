using MssDevLab.Common.Models;

namespace MssDevLab.VkService.Services
{
    public interface ISearchService
    {
        Task<SearchCompletedEvent> FetchDataAsync(SearchRequestedEvent requestData);
    }
}