namespace RentOffice.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; internal set; }
        public string? Email { get; internal set; }
        public string Password { get; internal set; }
        public string? Role { get; internal set; }
        public DateOnly CreatedAt { get; internal set; }
    }
}
