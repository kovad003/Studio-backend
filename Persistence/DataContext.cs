using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;
public class DataContext : IdentityDbContext<User, Role, string>
{
    public DataContext(DbContextOptions options) : base(options) { }

    public DbSet<Project> Projects { get; set; }
    
    // protected override void OnModelCreating(ModelBuilder builder)
    // {
    //     builder.Entity<Role>()
    //         .HasData(
    //             new Role { Name = "Admin", NormalizedName = "ADMIN" },
    //             new Role { Name = "Assistant", NormalizedName = "ASSISTANT" },
    //             new Role { Name = "Client", NormalizedName = "CLIENT" }
    //         );
    // }
}
