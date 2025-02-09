using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RentOffice.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Slug { get; set; }
        public DateOnly CreatedAt { get; set; }

        [JsonIgnore]
        public ICollection<OfficeSpace> OfficeSpaces { get; set; }
    }
}
