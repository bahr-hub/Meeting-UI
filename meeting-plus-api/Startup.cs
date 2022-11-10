using AutoMapper;
using ElmahCore.Mvc;
using MeetingPlus.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Middlewares;
using Hangfire;
using Hangfire.SqlServer;
using Services;
using WebApp;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Syncfusion.Licensing;

namespace MeetingPlus.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //var appSettingsSection = Configuration.GetSection("AppSettings");
            //Register Syncfusion license
            

            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(Configuration.GetConnectionString("SystemConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true
            }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<BaseDataBaseContext>(options => options
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies(false)
                .UseSqlServer(Configuration.GetConnectionString("SystemConnection")

            ));

            // configure DI for application services
            services.AddDIService();

            services.AddAutoMapper(typeof(Startup).Assembly);

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = appSettingsSection["ClientApp"];
            });
            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                          .GetBytes(Configuration.GetSection("AppSettings:Secret").Value)),
                      ValidateIssuer = false,
                      ValidateAudience = false,
                  };
                  options.Events = new JwtBearerEvents
                  {
                      OnMessageReceived = context =>
                      {
                          var accessToken = context.Request.Query["access_token"];

                          // If the request is for our hub...
                          var path = context.HttpContext.Request.Path;
                          string[] urls = { "/notify", "/Redirect", "/anonymous" };
                          bool hasAccess = false;
                          foreach (var url in urls)
                          {
                              hasAccess = path.StartsWithSegments(url);
                              if (hasAccess)
                              {
                                  context.Token = accessToken;

                                  break;
                              }
                          }
                          if (!string.IsNullOrEmpty(accessToken))
                          {
                              if (path.StartsWithSegments("/anonymous"))
                                  context.Token = accessToken;
                          }
                          return Task.CompletedTask;
                      }
                  };
              });

            services.AddSwaggerDocument(c =>
            {
                c.Version = "v1";
                c.Title = "System API";
                c.Description = "ASP.NET Core Web API";
                c.AddSecurity("Bearer", new List<string>(), new NSwag.OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                });
            });
            services.AddElmah();


            services.AddDbContextFactory<BaseDataBaseContext>(Configuration.GetConnectionString("SystemConnection"));
            services.AddSingleton<InventoryDatabaseSubscription, InventoryDatabaseSubscription>();
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, MyCustomProvider>();
            //services.AddScoped<IHubContext<NotificationHub>, IHubContext<NotificationHub>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IHostingEnvironment env)
        {
            //Register Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense("@31392e342e30m2pTezvPJjjK/RwH7Jw3CB5lilfEveVB6Djp/6XJub8=");
             
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<NotificationHub>("/notify");
            //});
            app.UseSignalR(hubs =>
            {
                hubs.MapHub<NotificationHub>("/notify");
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<MeetingHub>("/MeetingHub");
                routes.MapHub<HomeHub>("/HomeHub");
            });

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(); // serve Swagger UI
            app.UseReDoc(); // serve ReDoc UI

            app.UseElmah();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseHangfireDashboard();
            RecurringJob.AddOrUpdate<IMeetingReminderService>(x => x.GetParticipants(), "*/2 * * * *");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "Images/Users")),
                RequestPath = "/Images/Users"
            });
            //Enable directory browsing
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "Images/Users")),
                RequestPath = "/Images/Users"
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "meetingplusAngular";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    spa.Options.StartupTimeout = new TimeSpan(0, 5, 0);
                }
            });

            app.UseSqlTableDependency<InventoryDatabaseSubscription>(Configuration.GetConnectionString("SystemConnection"));

            app.SeedDatabase();
        }
    }
}
