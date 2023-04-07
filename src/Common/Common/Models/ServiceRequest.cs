using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MssDevLab.Common.Models
{
    public class ServiceRequest
    {
        public string? QueryString { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
    }
}
