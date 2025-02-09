namespace RentOffice.Payload.Request
{
     public class CityRequest
    {
        public string Name { get; set; }
        public IFormFile? PhotoFile { get; set; }
    }
}
