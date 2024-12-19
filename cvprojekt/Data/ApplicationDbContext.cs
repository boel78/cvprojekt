using cvprojekt.Models;
using Microsoft.EntityFrameworkCore;

namespace cvprojekt.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        
        }

        public DbSet<User> Users { get; set; }
    }
}
