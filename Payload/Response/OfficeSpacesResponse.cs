using RentOffice.Models;

namespace RentOffice.Payload.Response
{
    public class OfficeSpacesResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public List<OfficeSpace>? ListData { get; set; }
        public OfficeSpace? Data { get; set; }
    }
}
