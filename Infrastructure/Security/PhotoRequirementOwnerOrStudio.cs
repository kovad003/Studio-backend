using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class PhotoRequirementOwnerOrStudio : IAuthorizationRequirement
{
    public string[] Roles { get; set; }
}

public class PhotoOwnershipRequirementHandler : AuthorizationHandler<ProjectRequirementOwnerOrStudio>
{
    private readonly DataContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PhotoOwnershipRequirementHandler(
        DataContext dbContext, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext authContext, ProjectRequirementOwnerOrStudio requirement)
    {
        // Getting UserID
        var userId = authContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine("current USER_ID: " + userId);
        if (userId == null) return Task.CompletedTask;

        // Getting PhotoID
        var photoId = _httpContextAccessor
            .HttpContext?.Request.RouteValues.SingleOrDefault(
            x => x.Key == "id").Value?.ToString();
        Console.WriteLine("PHOTO_ID: " + photoId); 
        
        // Getting Photo
        var photo = _dbContext.Photos
            .Include(p => p.Project)
            .FirstOrDefaultAsync(p => p.Id.Equals(photoId));
        if (photo == null) return null;

        var project = photo.Result.Project;
        if (project != null) // a project where OwnerID == UserID
            authContext.Succeed(requirement);

        // Checking User Role
        foreach (var role in requirement.Roles)
        {
            Console.WriteLine("HAS_USER_ROLE -> " + authContext.User.IsInRole(role));
            if (authContext.User.IsInRole(role)) authContext.Succeed(requirement);
        }
        // Console.WriteLine("HAS_USER_ROLE -> " + hasUserRole);
        
        return Task.CompletedTask;
    }
}
