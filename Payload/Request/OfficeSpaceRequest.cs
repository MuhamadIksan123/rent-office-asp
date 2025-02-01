namespace RentOffice.Payload.Request
{
    // Request model to match incoming data
    public class OfficeSpaceRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Duration { get; set; }
        public int Price { get; set; }
        public string Thumbnail { get; set; }
        public string About { get; set; }
        public int CityId { get; set; }
        public List<string> Photos { get; set; }
        public List<string> Benefits { get; set; }
    }
}
