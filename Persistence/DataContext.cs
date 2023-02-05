using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;
public class DataContext : IdentityDbContext<User, Role, string>
{
    public DataContext(DbContextOptions options) : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // If project is deleted derived photos will be deleted as well.
        // builder.Entity<Photo>()
        //     .HasOne(photo => photo.Project)
        //     .WithMany(project => project.Photos)
        //     .OnDelete(DeleteBehavior.Cascade);
        
        // If project is deleted derived comments will be deleted as well.
        builder.Entity<Comment>()
            .HasOne(comment => comment.Project)
            .WithMany(project => project.Comments)
            .OnDelete(DeleteBehavior.Cascade);
        
        // If project owner is deleted derived comments will be deleted as well.
        // builder.Entity<Project>()
        //     .HasOne(project => project.Owner)
        //     .WithMany(owner => owner.Projects)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}
