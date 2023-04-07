namespace MssDevLab.Common.Models
{
    public class ServiceData
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ServiceType Type { get; set; }
        public string? Url { get; set; }
        public int PageCount { get; set; }
    }
}