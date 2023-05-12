using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;
using MssDevLab.Common.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.SignalR;
using MssDevLab.WebMVC.Hubs;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger _logger;

        private readonly IHubContext<SearchHub> _hubContext;

        public NotificationService(ILogger<TestAdServiceIntegration> logger, IHubContext<SearchHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
            _logger.LogDebug("NotificationService instance created");
        }

        public async Task StartSearch(ServiceRequest request)
        {
            if (request.UserPreferences?.Email != null)
            {
                // TODO: load user data
            }
            // TODO: publish event to all subscribers

            // TODO: remove the next debug stub (modeling response)

            for (int i = 0; i < request.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = request.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.TestService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "http://www.mssdevlab.com/img/zoom.png",
                    Title = $"TestService index:{i + 1} page:{request.PageNumber}",
                    Description = $"Example of the data from provider. TestService email:'{request.UserPreferences?.Email}' query:'{request.QueryString}'",
                    Relevance = i,
                    ConnectionId = request.ConnectionId
                };
                await SearchCompleted(data);
            }
        }

        public async Task SearchCompleted(ServiceData data)
        {
            if (!string.IsNullOrWhiteSpace(data.ConnectionId))
            {
                await _hubContext.Clients.Client(data.ConnectionId).SendAsync("ReceiveMessage", data);
            }
        }
    }
}
