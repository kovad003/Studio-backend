using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<User>(settings =>
            {
                settings.Password.RequireNonAlphanumeric = false;
                settings.Password.RequireUppercase = true;
                settings.User.RequireUniqueEmail = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<DataContext>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        services.AddAuthorization(options =>
        {
            // Role-based Policies:
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ClientOnly", policy => policy.RequireRole("Client"));
            options.AddPolicy("AdminOrClient", policy => policy.RequireRole("Admin", "Client"));
            options.AddPolicy("AdminOrAssistant", policy => policy.RequireRole("Admin", "Assistant"));

            // Custom / Combined Policies & Requirements:
            options.AddPolicy("OwnerOnly", policy =>
            {
                policy.Requirements.Add(new RoleOrOwnershipRequirement());
            });
            options.AddPolicy("OwnerOrStudio", policy => 
                policy.Requirements.Add(
                new RoleOrOwnershipRequirement()
                {
                    Roles = new []{"Admin", "Assistant"}
                }));
        });
        services.AddTransient<IAuthorizationHandler, OwnershipRequirementHandler>();
        services.AddScoped<TokenService>();

        return services;
    }
}