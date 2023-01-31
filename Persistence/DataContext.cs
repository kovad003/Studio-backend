using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;
public class DataContext : IdentityDbContext<User, Role, string>
{
    public DataContext(DbContextOptions options) : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Photo> Photos { get; set; }
    
    
    // Entity Relationships can be configured here manually if that is necessary:
    // protected override void OnModelCreating(ModelBuilder builder)
    // {
    //     base.OnModelCreating(builder);
    //     
    //     // configures one-to-many relationship
    //     builder.Entity<Project>()
    //         .HasOne(p => p.User)
    //         .WithMany(a => a.Projects);
    // }
    
    
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
