using InterLinked.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterLinked.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<InterlinkedAppUser>(options)
    {
        public DbSet<InterLinked.Models.Post> Post { get; set; } = default!;
    }
}
