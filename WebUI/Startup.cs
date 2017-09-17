using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EaisApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using WebUI.Data;
using WebUI.Data.Entities;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Pagination;
using WebUI.Models;
using WebUI.Resources;
using WebUI.Services;

namespace WebUI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("conf.json", optional: false, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            env.ConfigureNLog("nlog.config");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Cookies.ApplicationCookie.AutomaticChallenge = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddPager();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc(options => options.ModelBinderProviders.Insert(0, new DateModelBinderProvider()))
                .AddDataAnnotationsLocalization(
                    options => options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource)));

            services.AddSingleton<IConfiguration>(Configuration);

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // Configuring session
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.CookieHttpOnly = true;
            });


            // Add Eaisto api
            services.AddScoped<IUserStorage>(provider =>
                new SessionStorage(provider.GetRequiredService<IHttpContextAccessor>()
                                           .HttpContext.Session));
            services.AddScoped<EaistoApi>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
            //add NLog.Web
            app.AddNLogWeb();

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("ru-RU")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(options);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"]
            });

            if (Configuration["AutoMigrate"].ToLower() == "true" && !env.IsEnvironment("TEST"))
            {
                var logger = loggerFactory.CreateLogger<Startup>();
                logger.LogInformation("Start migration");
                using (
                    var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    //serviceScope.ServiceProvider.GetService<AppDbContext>().Database.EnsureDeleted();
                    //serviceScope.ServiceProvider.GetService<AppDbContext>().Database.EnsureCreated();
                    var dbc = serviceScope.ServiceProvider.GetService<AppDbContext>();
                    dbc.Database.Migrate();
                }
            }

            app.UseSession();

            var userManager = app.ApplicationServices.GetRequiredService<UserManager<User>>();
            SecurityRoutines.CreateAdminUser(userManager, Configuration["admin:login"],
                Configuration["admin:password"]).Wait();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Cards}/{action=Create}/{id?}");
            });
        }
    }
}
