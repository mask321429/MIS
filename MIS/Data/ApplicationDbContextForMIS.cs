using Microsoft.EntityFrameworkCore;
using MIS.Data.Models;

namespace MIS.Data
{
    public class ApplicationDbContextForMIS : DbContext
    {
        public ApplicationDbContextForMIS(DbContextOptions<ApplicationDbContextForMIS> options) : base(options) { }

        public ApplicationDbContextForMIS() { }
        public DbSet<Record> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ForMIS;Username=postgres;Password=12345");
        }
    }
}
