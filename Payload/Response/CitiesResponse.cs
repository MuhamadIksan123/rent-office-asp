using System.Text.Json.Serialization;
using RentOffice.Models;

namespace RentOffice.Payload.Response
{
    public class CitiesResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<City>? ListData { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public City? Data { get; set; }
    }
}
