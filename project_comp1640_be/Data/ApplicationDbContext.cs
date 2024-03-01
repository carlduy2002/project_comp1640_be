using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Model;

namespace project_comp1640_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Faculties> Faculties { get; set; }
        public DbSet<Contributions> Contributions { get; set; }
        public DbSet<Marketing_Comments> Marketing_Comments { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Academic_Years> Academic_Years { get; set; }
    }
}
