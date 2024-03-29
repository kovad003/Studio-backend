﻿using Domain;
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
        builder.Entity<User>()
            .HasMany(user => user.Projects)
            .WithOne(project => project.Owner)
            .OnDelete(DeleteBehavior.Cascade);
        
        // If project owner is deleted derived comments will be deleted as well.
            builder.Entity<Project>()
            .HasOne(project => project.Owner)
            .WithMany(owner => owner.Projects)
            .OnDelete(DeleteBehavior.Cascade);
        
        // If project is deleted derived photos will be deleted as well.
        builder.Entity<Photo>()
            .HasOne(photo => photo.Project)
            .WithMany(project => project.Photos)
            .OnDelete(DeleteBehavior.Cascade);
        
        // If project is deleted derived comments will be deleted as well.
        builder.Entity<Comment>()
            .HasOne(comment => comment.Project)
            .WithMany(project => project.Comments)
            .OnDelete(DeleteBehavior.Cascade);
        

        // public override int SaveChanges()
        // {
        //     // Get a list of all deleted photos
        //     var deletedPhotos = ChangeTracker.Entries<Photo>()
        //         .Where(e => e.State == EntityState.Deleted)
        //         .Select(e => e.Entity)
        //         .ToList();
        //         
        //     // Delete the corresponding images from Cloudinary for each deleted photo
        //     foreach (var photo in deletedPhotos)
        //     {
        //         // DeleteImageFromCloudinary(photo.Url);
        //     }
        //         
        //     // Call the base implementation of SaveChanges to persist the changes to the database
        //     return base.SaveChanges();
        // }
        
    }
}
