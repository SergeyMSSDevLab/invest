using MssDevLab.Common.Models;

namespace MssDevLab.TestAdService.Services
{
    public interface INotificationService
    {
        void PublishSearchResult(SearchCompletedEvent searchData);
    }
}