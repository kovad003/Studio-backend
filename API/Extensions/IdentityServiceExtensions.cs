using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

                // Lockout settings
                settings.Lockout.AllowedForNewUsers = true;
                settings.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                settings.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddRoles<Role>()
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<DataContext>();
            

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "YourCookieName";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                // Authenticating to SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            })
            .AddIdentityCookies();
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
                policy.Requirements.Add(new ProjectRequirementOwnerOrStudio());
            });
            options.AddPolicy("ProjectOwnerOrStudio", policy => 
                policy.Requirements.Add(
                new ProjectRequirementOwnerOrStudio()
                {
                    Roles = new []{"Admin", "Assistant"}
                }));
            // options.AddPolicy("PhotoOwnerOrStudio", policy => 
            //     policy.Requirements.Add(
            //     new PhotoRequirementOwnerOrStudio()
            //     {
            //         Roles = new []{"Admin", "Assistant"}
            //     }));
        });
        services.AddTransient<IAuthorizationHandler, OwnershipRequirementHandler>();
        services.AddScoped<TokenService>();

        return services;
    }
}