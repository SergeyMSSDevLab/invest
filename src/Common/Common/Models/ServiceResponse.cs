using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MssDevLab.Common.Models
{
    public class ServiceResponse
    {
        public IEnumerable<ServiceData>? Items { get; set; }
        public string? QueryString { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? ItemsAmount { get; set; }
    }
}
