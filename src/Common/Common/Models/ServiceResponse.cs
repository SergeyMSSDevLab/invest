using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MssDevLab.Common.Models
{
    public class ServiceResponse
    {
        public ServiceResponse() 
        { 
            Items = Array.Empty<ServiceData>();
        }
        public ServiceData[] Items { get; set; }
        public string? QueryString { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? ItemsAmount { get; set; }
        public ServiceType ServiceType { get; set; }
        public bool IsSuccesfull { get; set; }
    }
}
