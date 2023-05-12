using Elasticsearch.Net;
using Microsoft.AspNetCore.SignalR;
using MssDevLab.Common.Models;
using MssDevLab.WebMVC.Services;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MssDevLab.WebMVC.Hubs
{
    public class SearchHub : Hub
    {
        private readonly INotificationService _notificationService;
        public SearchHub(INotificationService notificationService) 
        { 
            _notificationService = notificationService;
        }

        public async Task Search(string searchString, int pageNumber)
        {
            var connectionId = Context.ConnectionId;
            var userName = Context.User?.Identity?.Name;
            var serviceRequest = new ServiceRequest
            {
                PageNumber = pageNumber,
                PageSize = 10,
                QueryString = searchString ?? string.Empty,
                UserPreferences = null,
                ConnectionId = connectionId
            };

            if (!string.IsNullOrWhiteSpace(userName))
            {
                serviceRequest.UserPreferences = new UserData { Email = userName };
            }
            await _notificationService.StartSearch(serviceRequest);
        }
    }
}
