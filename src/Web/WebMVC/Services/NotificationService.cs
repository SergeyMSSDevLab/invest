using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;
using MssDevLab.Common.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.SignalR;
using MssDevLab.WebMVC.Hubs;
using System.Threading.Tasks;
using MssDevLab.CommonCore.Interfaces.EventBus;
using MssDevLab.CommonCore.Models;

namespace MssDevLab.WebMVC.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger _logger;
        private readonly IHubContext<SearchHub> _hubContext;
        private readonly IEventBus _eventBus;

        public NotificationService(ILogger<NotificationService> logger, 
            IHubContext<SearchHub> hubContext, 
            IEventBus eventBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public void StartSearch(SearchRequestedEvent request)
        {
            _logger.LogDebug("NotificationService StartSearch started");
            if (request.UserPreferences?.Email != null)
            {
                // TODO: load user data
            }

            var evt = (IntegrationEvent)request;
            try
            {
                _eventBus.Publish(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing search requested event: {IntegrationEventId} event: ({@IntegrationEvent}. Query string: {query})", evt.Id, evt, request.QueryString);
            }
        }

        public async Task SearchCompletedAsync(SearchCompletedEvent eventData)
        {
            _logger.LogDebug("NotificationService SearchCompletedAsync started");
            // TODO: consider to send all response and enumerate on the client side
            if (!string.IsNullOrWhiteSpace(eventData.ConnectionId) && eventData.IsSuccesfull)
            {
                foreach(var data in eventData.Items)
                {
                    await _hubContext.Clients.Client(eventData.ConnectionId).SendAsync("ReceiveMessage", data);
                }
            }
            else
            {
                _logger.LogError("ERROR searching: eventId={IntegrationEventId} event=({@IntegrationEvent})", eventData.Id, eventData);
            }
        }
    }
}
