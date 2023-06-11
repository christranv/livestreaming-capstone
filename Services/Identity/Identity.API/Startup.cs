using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Identity.API.ExtensionGrant;
using Identity.API.Providers;
using Identity.API.Repositories;
using Identity.API.Repositories.Interfaces;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Identity.API.Configuration;
using Identity.API.Data;
using Identity.API.Filters;
using Identity.API.Infrastructure;
using Identity.API.IntegrationEvents;
using Identity.API.Models;
using Identity.API.Services.Interfaces;
using Identity.API.Services;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using Team5.BuildingBlocks.MessageBroker.EventBus;
using Team5.BuildingBlocks.MessageBroker.EventBus.Abstractions;
using Team5.BuildingBlocks.MessageBroker.EventBusRabbitMQ;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF;
using Team5.BuildingBlocks.MessageBroker.IntegrationEventLogEF.Services;

namespace Identity.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 21)),
                    mySqlOptions =>
                    {
                        mySqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        mySqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 21)),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<AppSettings>(Configuration);

            // if (Configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            // {
            //     services.AddDataProtection(opts =>
            //     {
            //         opts.ApplicationDiscriminator = "string.identity";
            //     })
            //     .PersistKeysToRedis(ConnectionMultiplexer.Connect(Configuration["DPConnectionString"]), "DataProtection-Keys");
            // }

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddMySql(
                    Configuration["ConnectionString"],
                    name: "IdentityDB-check",
                    tags: new string[] {"identitydb"});

            services.AddTransient<ILoginService<ApplicationUser>, EfLoginService>();
            services.AddTransient<IRedirectService, RedirectService>();

            var connectionString = Configuration["ConnectionString"];
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Adds IdentityServer
            services.AddIdentityServer(options =>
                {
                    options.IssuerUri = "null";
                    options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                    options.EmitStaticAudienceClaim = true;
                })
                // .AddSigningCredential(Certificate.Certificate.Get())
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(connectionString,
                        new MySqlServerVersion(new Version(8, 0, 21)),
                        mySqlOptions =>
                        {
                            mySqlOptions.MigrationsAssembly(migrationsAssembly);
                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            mySqlOptions.EnableRetryOnFailure(maxRetryCount: 15,
                                maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(connectionString,
                        new MySqlServerVersion(new Version(8, 0, 21)),
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(migrationsAssembly);
                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });
                })
                .Services.AddTransient<IProfileService, ProfileService>();

            services.AddTransient<IExtendedProfileService, ProfileService>();
            services.AddTransient<IUserFileService, UserFileService>();

            // Add External Authentication for IS4
            services.AddScoped<IExtensionGrantValidator, ExternalAuthenticationGrant>();
            // Add External Provider repository
            services.AddScoped<IProviderRepository, ProviderRepository>();

            services.AddHttpClient<IGoogleAuthProvider, GoogleAuthProvider<ApplicationUser>>();

            services.AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); });
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddIntegrationServices(Configuration);
            services.AddEventBus(Configuration);
            
            // Automapper config
            IMapper mapper = AutoMapperConfig.Configure().CreateMapper();
            services.AddSingleton(mapper);

            var container = new ContainerBuilder();
            container.Populate(services);
            
            return new AutofacServiceProvider(container.Build());
        }

        /// <summary>
        /// Add roles to IdentityServer4 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminRoleExists = await roleManager.RoleExistsAsync("admin");
            var userRoleExists = await roleManager.RoleExistsAsync("user");

            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            var userToMakeAdmin = await userManager.FindByNameAsync("admin@team5.com");
            var userToMakeUser = await userManager.FindByNameAsync("test@microsoft.com");
            await userManager.AddToRoleAsync(userToMakeAdmin, "admin");
            await userManager.AddToRoleAsync(userToMakeUser, "user");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IServiceProvider services)
        {
            CreateUserRoles(services).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{PathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");
            app.UseStaticFiles();

            // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                await next();
            });

            app.UseForwardedHeaders();
            // Adds IdentityServer
            app.UseIdentityServer();

            // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure", 
            // the coockies shold be expided from https, but in eShop, the internal comunicacion in aks and docker compose is http.
            // To avoid this problem, the policy of cookies shold be in Lax mode.
            app.UseCookiePolicy(new CookiePolicyOptions {MinimumSameSitePolicy = SameSiteMode.Lax});
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }


    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => c => new IntegrationEventLogService(c));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IIdentityService, IdentityService>();

            services.AddTransient<IIdentityIntegrationEventService, IdentityIntegrationEventService>();

            // Config RabbitMQ
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
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

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            services.AddLocalApiAuthentication();

            return services;
        }
        
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope,
                    eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            //services.AddTransient<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
            //services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>();

            return services;
        }
    }
}