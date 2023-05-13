using MssDevLab.Common.Models;
using System;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    public interface INotificationService
    {
        void StartSearch(SearchRequestedEvent request);

        Task SearchCompletedAsync(SearchCompletedEvent eventData);
    }
}