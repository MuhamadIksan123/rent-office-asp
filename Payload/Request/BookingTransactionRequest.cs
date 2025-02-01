using System.ComponentModel.DataAnnotations;

namespace RentOffice.Payload.Request
{
    public class BookingTransactionRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        [Required]
        public DateOnly StartedAt { get; set; }
        [Required]
        public int TotalAmount { get; set; }
        [Required]
        public int OfficeSpaceId { get; set; }
    }
}
