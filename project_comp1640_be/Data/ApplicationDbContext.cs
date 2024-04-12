using Microsoft.EntityFrameworkCore;
using project_comp1640_be.Model;
using System.Data;

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
        public DbSet<Page_Views> Page_Views { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Faculties>().ToTable("Faculties");
            modelBuilder.Entity<Contributions>().ToTable("Contributions");
            modelBuilder.Entity<Marketing_Comments>().ToTable("Marketing_Comments");
            modelBuilder.Entity<Roles>().ToTable("Roles");
            modelBuilder.Entity<Academic_Years>().ToTable("Academic_Years");
            modelBuilder.Entity<Page_Views>().ToTable("Page_Views");

            modelBuilder.Entity<Marketing_Comments>()
                .HasOne(m => m.users)
                .WithMany(u => u.Marketing_Comments)
                .HasForeignKey(m => m.comment_user_id)
                .OnDelete(DeleteBehavior.NoAction);
        }

        
    }
}
