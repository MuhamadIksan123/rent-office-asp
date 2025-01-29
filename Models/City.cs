namespace RentOffice.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string Slug { get; set; }
        public DateOnly CreatedAt { get; set; }

        public void setNameAttribute(string value)
        {
            Name = value;
            Slug = value.ToLower().Replace(" ", "-");
        }

        public ICollection<OfficeSpace> OfficeSpaces { get; set; }
    }
}
