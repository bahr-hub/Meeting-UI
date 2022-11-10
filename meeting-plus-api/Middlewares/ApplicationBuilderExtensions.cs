using MeetingPlus.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Models;
using Services;
using Shared;
using System;

namespace WebApp.Middlewares
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            IServiceProvider serviceProvider = app.ApplicationServices.CreateScope().ServiceProvider;
            try
            {
                IOptions<AppSettings> appSettings = serviceProvider.GetService<IOptions<AppSettings>>();
                BaseDataBaseContext Context = serviceProvider.GetService<BaseDataBaseContext>();
                if (appSettings.Value.ToSeed)
                    SeedService.Seed(Context);
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
            return app;
        }

        public static void UseSqlTableDependency<T>(this IApplicationBuilder services, string connectionString)
    where T : IDatabaseSubscription
        {
            var serviceProvider = services.ApplicationServices;
            var subscription = serviceProvider.GetService<T>();
            subscription.Configure(connectionString);
        }
    }
}
