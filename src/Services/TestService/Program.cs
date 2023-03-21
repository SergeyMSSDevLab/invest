
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace MssDevLab.TestService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

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