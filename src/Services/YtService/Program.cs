
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using MssDevLab.Common.Extensions;
using MssDevLab.Common.Models;
using MssDevLab.CommonCore.Interfaces.EventBus;
using Microsoft.AspNetCore.Connections;
using MssDevLab.CommonCore.Services;
using MssDevLab.EventBusRabbitMQ;
using RabbitMQ.Client;
using MssDevLab.YtService.Services;

namespace MssDevLab.YtService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureElasticSerilog("YtService");

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddEventBus(builder.Configuration);

            builder.Services.AddTransient<ISearchService, SearchService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseAuthorization();
            app.UseEventBus();

            app.MapControllers();

            app.Run();
        }

        private static void UseEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<SearchRequestedEvent, IIntegrationEventHandler<SearchRequestedEvent>>();
        }

        private static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var retryCount = 5;
            if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
            {
                if (!int.TryParse(configuration["EventBusRetryCount"], out retryCount))
                {
                    retryCount = 5;
                }
            }

            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = configuration["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusRabbitMQ.EventBusRabbitMQ(rabbitMQPersistentConnection, logger, sp, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient< IIntegrationEventHandler<SearchRequestedEvent>, SearchRequestedEventHandler >();

            services.AddSingleton<IRabbitMQPersistentConnection>(sp => {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }
    }
}