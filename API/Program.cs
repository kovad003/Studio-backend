using API.Extensions;
using API.Middleware;
using API.SignalR;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment.EnvironmentName;

// Add services to the container.
// (Dependency Injection)
builder.Services.AddControllers(option =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    option.Filters.Add(new AuthorizeFilter(policy));
});

builder.Configuration.AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{env}.json", optional: true);

// Debugging
Console.WriteLine("debug: Environment: {0}", env);
foreach (var c in builder.Configuration.AsEnumerable())
{
    Console.WriteLine("debug: enum config: " + c.Key + " = " + c.Value);
}
// end

builder.Services.AddServiceExtensions(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
// (Middleware)
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager, roleManager);
} catch (Exception exception)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(exception, "!ERROR: An exception has occured during the process of migration!");
}

app.Run();
