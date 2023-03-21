using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MssDevLab.WebMVC.Data;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;

namespace MssDevLab.WebMVC
{
    public class Program
    {
        public static /*async Task*/ void Main(string[] args)
        {
            ConfigureLogging();

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            // Apply database migration automatically. Note that this approach is not
            // recommended for production scenarios. Consider generating SQL scripts from
            // migrations instead.
            //using (var scope = app.Services.CreateScope())
            //{
            //    await SeedData.EnsureSeedData(scope, app.Configuration, app.Logger);
            //}

            app.Run();
        }

        private static void ConfigureLogging()
        {
	        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
	        var configuration = new ConfigurationBuilder()
		        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		        .AddJsonFile(
			        $"appsettings.{environment}.json",
			        optional: true)
		        .Build();

            var uriSink = new Uri(configuration["ElasticConfiguration:Uri"] ?? "http://invest-ek:9200");
	        var elSinkOptions =  new ElasticsearchSinkOptions(uriSink)
	        {
		        AutoRegisterTemplate = true,
		        IndexFormat = $"invest-{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{environment.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
	        };

	        var loggerConfig = new LoggerConfiguration()
		        .Enrich.FromLogContext()
		        .Enrich.WithMachineName()
		        .WriteTo.Debug()
		        .WriteTo.Console()
		        .WriteTo.Elasticsearch(elSinkOptions)
		        .Enrich.WithProperty("Environment", environment)
		        .ReadFrom.Configuration(configuration);
            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}