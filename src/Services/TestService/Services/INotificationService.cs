using MssDevLab.Common.Models;

namespace MssDevLab.TestService.Services
{
    public interface INotificationService
    {
        void PublishSearchResult(SearchCompletedEvent searchData);
    }
}