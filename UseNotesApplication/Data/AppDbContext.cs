using Microsoft.EntityFrameworkCore;
using UseNotesApplication.Models;
namespace UseNotesApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<NoteVersion> NoteVersions { get; set; }
        public DbSet<UserPictures> UserPictures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Unique Constraint for UserName
            modelBuilder.Entity<Users>().HasIndex(x => x.UserName).IsUnique();
        }
    }
}
