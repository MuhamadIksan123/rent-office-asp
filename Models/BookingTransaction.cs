using RentOffice.Data;

namespace RentOffice.Models
{
    public class BookingTransaction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string BookingTrxId { get; set; }
        public string Slug { get; set; }
        public bool IsPaid { get; set; }
        public DateOnly StartedAt { get; set; }
        public int TotalAmount { get; set; }
        public int Duration { get; set; }
        public DateOnly EndedAt { get; set; }
        public int OfficeSpaceId { get; set; } // Foreign Key
        public DateOnly CreatedAt { get; set; }

        public virtual OfficeSpace OfficeSpace { get; set; }

        public static string GenerateUniqueTrxId(ApplicationDbContext context)
        {
            string prefix = "FO";
            string randomString;
            Random rand = new Random();

            do
            {
                randomString = prefix + rand.Next(1000, 10000).ToString();
            } while (context.BookingTransactions.Any(bt => bt.BookingTrxId == randomString));

            return randomString;
        }

    }
}
