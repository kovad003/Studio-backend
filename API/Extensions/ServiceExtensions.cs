using Application.Core;
using Application.Projects;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
            });
        });
        services.AddMediatR(typeof(List.Handler));
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Create>();

        return services;
    }
}