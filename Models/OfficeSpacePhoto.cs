namespace RentOffice.Models
{
    public class OfficeSpacePhoto
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
