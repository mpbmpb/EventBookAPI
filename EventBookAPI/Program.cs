using System.Threading.Tasks;
using EventBookAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventBookAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var serviceScope = host.Services.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

            await dbContext.Database.MigrateAsync();

            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("admin"))
            {
                var adminRole = new IdentityRole("admin");
                await roleManager.CreateAsync(adminRole);
            }

            if (!await roleManager.RoleExistsAsync("user"))
            {
                var userRole = new IdentityRole("user");
                await roleManager.CreateAsync(userRole);
            }
        }
            
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}