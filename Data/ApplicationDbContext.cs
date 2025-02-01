using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using RentOffice.Models;

namespace RentOffice.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BookingTransaction> BookingTransactions { get; set; }
        public DbSet<OfficeSpace> OfficeSpaces { get; set; }
        public DbSet<OfficeSpaceBenefit> OfficeSpaceBenefits { get; set; }
        public DbSet<OfficeSpacePhoto> OfficeSpacePhotos { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add any additional configurations for your models here if needed

            modelBuilder.Entity<BookingTransaction>()
                .HasOne(a => a.OfficeSpace)
                .WithMany(b => b.BookingTransactions)
                .HasForeignKey(c => c.OfficeSpaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OfficeSpace>()
                .HasOne(a => a.City)
                .WithMany(b => b.OfficeSpaces)
                .HasForeignKey(c => c.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
