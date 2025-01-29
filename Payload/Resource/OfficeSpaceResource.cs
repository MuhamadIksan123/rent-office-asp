namespace RentOffice.Payload.Resource
{
    public class OfficeSpaceResource
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
        public int? CityId { get; set; }
        public DateOnly CreatedAt { get; set; }
        public IEnumerable<PhotoDTO> Photos { get; set; }
        public IEnumerable<BenefitDTO> Benefits { get; set; }
    }

    public class PhotoDTO
    {
        public string Photo { get; set; }
        public DateOnly CreatedAt { get; set; }
    }

    public class BenefitDTO
    {
        public string Name { get; set; }
        public DateOnly CreatedAt { get; set; }
    }

}
