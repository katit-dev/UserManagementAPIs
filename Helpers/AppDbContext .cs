using Microsoft.EntityFrameworkCore;
using UserManagementApi.Models;

namespace UserManagementApi.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options) {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // lay connection string tu appsettings.json
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ProductStoreConnection"));
        }

        public DbSet<User> Users { get; set; }
    }
}