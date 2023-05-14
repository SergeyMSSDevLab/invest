using MssDevLab.Common.Models;

namespace MssDevLab.VkService.Services
{
    public interface INotificationService
    {
        void PublishSearchResult(SearchCompletedEvent searchData);
    }
}