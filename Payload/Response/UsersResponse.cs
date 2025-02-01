using System.Text.Json.Serialization;
using RentOffice.Models;

namespace RentOffice.Payload.Response
{
    public class UsersResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<User>? ListData { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public User? Data { get; set; }
    }
}
