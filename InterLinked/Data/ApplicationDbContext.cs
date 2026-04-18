using InterLinked.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterLinked.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<InterlinkedAppUser>(options)
    {
        public DbSet<Post> Post { get; set; } = default!;
        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.MyPosts)
                .HasForeignKey(p => p.UserId);


            builder.Entity<Application>()
            .HasOne(a => a.User)
            .WithMany(u => u.MyApplications)
            .HasForeignKey(a => a.UserId);

            builder.Entity<Application>()
            .HasOne(a => a.Post)
            .WithMany(p => p.Applications)
            .HasForeignKey(a => a.PostId);
        }

    }
}
