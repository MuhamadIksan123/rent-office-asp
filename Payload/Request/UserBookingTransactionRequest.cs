using System;
using System.ComponentModel.DataAnnotations;

namespace RentOffice.Models
{
    public class UserBookingTransactionRequest
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateOnly StartedAt { get; set; }

        [Required]
        public int OfficeSpaceId { get; set; }

        [Required]
        public int TotalAmount { get; set; }
    }
}
