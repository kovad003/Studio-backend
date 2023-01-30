using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class OwnershipRequirement : IAuthorizationRequirement
{
}

public class OwnershipRequirementHandler : AuthorizationHandler<OwnershipRequirement>
{
    private readonly DataContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OwnershipRequirementHandler(
        DataContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext authContext, OwnershipRequirement requirement)
    {
        // Getting UserID
        var userId = authContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Console.WriteLine("current USER_ID: " + userId);
        if (userId == null) return Task.CompletedTask;

        // Getting ProjectID
        var projectId = Guid.Parse(_httpContextAccessor
            .HttpContext?.Request.RouteValues.SingleOrDefault(
            x => x.Key == "id").Value?.ToString());
        Console.WriteLine("PROJECT_ID: " + projectId);
        
        // 
        var project = _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Owner.Id == userId && x.Id == projectId)
            .Result;

        if (project == null) return Task.CompletedTask;

        authContext.Succeed(requirement);
        return Task.CompletedTask;
    }
}