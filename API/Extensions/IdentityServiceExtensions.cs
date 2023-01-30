using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
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
            options.AddPolicy("OwnerOfProject", policy =>
            {
                policy.Requirements.Add(new OwnershipRequirement());
            });
        });
        services.AddTransient<IAuthorizationHandler, OwnershipRequirementHandler>();
        services.AddScoped<TokenService>();

        return services;
    }
}