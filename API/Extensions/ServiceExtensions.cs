using System.Reflection;
using Application.Core;
using Application.Interfaces;
using Application.Projects;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(
        this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<DataContext>(opt =>
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            string env = environment.EnvironmentName;
            // Console.WriteLine("debug: Environment: {0}", env);
            // Console.WriteLine("debug: Connection String: {0}", connStr);

            if (env == "Production")
            {
                opt.UseNpgsql(connStr);
            }
            else
            {
                opt.UseSqlite(connStr);
            }
        });
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000");
            });
        });
        // services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddMediatR(typeof(List.Handler));
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Create>();
        services.AddHttpContextAccessor();
        services.AddTransient<UserAccessor>();
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddScoped<IPhotoAccessor, PhotoAccessor>();
        // String must match with attribute within appsettings.json:
        services.Configure<PhotoCloudSettings>(configuration.GetSection("Cloudinary"));
        services.AddSignalR();

        return services;
    }
}