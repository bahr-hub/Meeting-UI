using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Middlewares
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDIService(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IProposalService, ProposalService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IAbilityService, AbilityService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IVacationService, VacationService>();
            services.AddScoped<IMeetingService, MeetingService>();
            services.AddScoped<IMeetingTaskService, MeetingTaskService>();
            services.AddScoped<IMeetingTopicService, MeetingTopicService>();
            services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
            services.AddScoped<IMeetingReminderService, MeetingReminderService>();
            services.AddScoped<IGoogleCalender, GoogleCalenderService>();
            services.AddScoped<ILiecenseService, LiecenseService>();

            return services;
        }
        public static void AddDbContextFactory<DataContext>(this IServiceCollection services, string connectionString)
    where DataContext : DbContext
        {
            services.AddScoped<Func<DataContext>>((ctx) =>
            {
                var options = new DbContextOptionsBuilder<DataContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                return () => (DataContext)Activator.CreateInstance(typeof(DataContext), options);
            });
        }
    }
}
