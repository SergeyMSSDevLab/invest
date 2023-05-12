using MssDevLab.Common.Models;
using System;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    public interface INotificationService
    {
        Task StartSearch(ServiceRequest request);

        Task SearchCompleted(ServiceData data);
    }
}