using System.ComponentModel.DataAnnotations.Schema;

namespace RentOffice.Models
{
    public class OfficeSpace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public string Address { get; set; }
        public string Slug { get; set; }
        public bool IsOpen { get; set; }
        public bool IsFullBooked { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
        public string About { get; set; }
        public int? CityId { get; set; } // Foreign Key
        public DateOnly CreatedAt { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public ICollection<OfficeSpacePhoto> Photos { get; set; }

        public ICollection<OfficeSpaceBenefit> Benefits { get; set; }

        public ICollection<BookingTransaction> BookingTransactions { get; set; } = new List<BookingTransaction>();

        // Override the OnModelCreating method to set up Slug field
        public void SetName(string value)
        {
            Name = value;
            Slug = value.ToLower().Replace(" ", "-");
        }

    }
}
