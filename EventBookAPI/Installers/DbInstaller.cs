using EventBookAPI.Data;
using EventBookAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBookAPI.Installers;

public class DbInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddDefaultIdentity<IdentityUser>(options =>
                options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IPageElementService, PageElementService>();
    }
}