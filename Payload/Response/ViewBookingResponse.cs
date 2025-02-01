using RentOffice.Models;

namespace RentOffice.Payload.Response
{
    public class ViewBookingResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public BookingTransaction? Data { get; set; }
    }
}
