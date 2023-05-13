using MssDevLab.CommonCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MssDevLab.Common.Models
{
    public record SearchRequestedEvent : IntegrationEvent
    {
        public string? ConnectionId { get; set; }
        public string? QueryString { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;

        public UserData? UserPreferences { get; set; }
    }
}
