using MssDevLab.Common.Models;

namespace MssDevLab.YtService.Services
{
    public interface INotificationService
    {
        void PublishSearchResult(SearchCompletedEvent searchData);
    }
}