using MssDevLab.Common.Models;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    public interface ITestAdServiceIntegration
    {
        Task<SearchCompletedEvent> FetchAds(SearchRequestedEvent request);
    }
}
