using MssDevLab.Common.Models;
using MssDevLab.CommonCore.Interfaces.Http;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    public interface ITestService1Integration
    {
        Task<SearchCompletedEvent> FetchData(SearchRequestedEvent request);
    }
}
