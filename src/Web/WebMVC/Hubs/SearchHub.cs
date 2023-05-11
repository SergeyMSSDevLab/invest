using Elasticsearch.Net;
using Microsoft.AspNetCore.SignalR;
using MssDevLab.Common.Models;
using System.Threading.Tasks;

namespace MssDevLab.WebMVC.Hubs
{
    public class SearchHub : Hub
    {
        public async Task Search(string searchString, int pageNumber)
        {
            var connectionId = Context.ConnectionId;
            var userName = Context.User?.Identity?.Name;
            var serviceRequest = new ServiceRequest
            {
                PageNumber = pageNumber,
                PageSize = 10,
                QueryString = searchString ?? string.Empty,
                UserPreferences = null
            };

            if (!string.IsNullOrWhiteSpace(userName))
            {
                serviceRequest.UserPreferences = new UserData { Email = userName };
            }
            // TODO: add connectionId to the service request
            // TODO: publish event to all subscribers

            // TODO: remove the next debug stub (modeling response)
            for (int i = 0; i < serviceRequest.PageSize; i++)
            {
                var data = new ServiceData
                {
                    Id = serviceRequest.PageNumber.ToString() + i.ToString(),
                    Type = ServiceType.TestService,
                    Url = "http://www.mssdevlab.com",
                    ImageUrl = "http://www.mssdevlab.com/img/zoom.png",
                    Title = $"TestService index:{i + 1} page:{serviceRequest.PageNumber}",
                    Description = $"Example of the data from provider. TestService email:'{userName}' query:'{serviceRequest.QueryString}'",
                    Relevance = i
                };
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", data);
            }
        }
    }
}
