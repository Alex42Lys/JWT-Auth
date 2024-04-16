using Microsoft.EntityFrameworkCore;

namespace WebApplication4
{
    public class ApplicationContext : DbContext
    {
        public DbSet<LoginData> LoginDataArr { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=.;Database=Test1;
                Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
