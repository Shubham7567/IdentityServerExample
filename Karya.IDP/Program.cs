using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Karya.IDP;
using Karya.IDP.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting Up");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lg) =>
{
    lg.MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(
            outputTemplate:
            "[(Timestamp:HH:mm:ss) {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
            theme: AnsiConsoleTheme.Code
        ).Enrich.FromLogContext();
});

var services = builder.Services;
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("Default");
var migrationAssembly = typeof(Config).Assembly.GetName().Name;

services.AddRazorPages();

services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString,
    sqlOptions => sqlOptions.MigrationsAssembly(migrationAssembly)));

services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    options.EmitStaticAudienceClaim = true;
}
).AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
    opt => opt.MigrationsAssembly(migrationAssembly)))
    .AddOperationalStore(options => options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
    opt => opt.MigrationsAssembly(migrationAssembly)))
    .AddAspNetIdentity<IdentityUser>();
services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseStaticFiles();
app.UseIdentityServer();
app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapRazorPages());


    Log.Information("Seeding database....");
    SeedData.EnsureSeedData(connectionString);
    Log.Information("Done seeding database, Exiting.....");

app.Run();
