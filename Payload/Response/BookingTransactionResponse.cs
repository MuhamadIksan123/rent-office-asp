using System.Text.Json.Serialization;
using RentOffice.Models;

namespace RentOffice.Payload.Response
{
    public class BookingTransactionResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<BookingTransaction>? ListData { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BookingTransaction? Data { get; set; }
    }
}
