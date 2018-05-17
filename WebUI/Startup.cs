using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EaisApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using WebUI.Data;
using WebUI.Data.Entities;
using WebUI.Infrastructure;
using WebUI.Infrastructure.Pagination;
using WebUI.Resources;
using WebUI.Services;
using WebUI.Data;
using WebUI.Models;
using WebUI.Services;

namespace WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    //options.Cookies.ApplicationCookie.AutomaticChallenge = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            services.AddPager();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc(options =>
                {
                    options.Filters.Add<ExceptionFilter>();
                    options.ModelBinderProviders.Insert(0, new DateModelBinderProvider());
                })
                .AddDataAnnotationsLocalization(
                    options => options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource)));

            services.AddSingleton<IConfiguration>(Configuration);

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddSingleton(new CardDocxGenerator(
                Path.Combine(Environment.ContentRootPath, Configuration["CardTemplatePath"]),
                Path.Combine(Environment.ContentRootPath, Configuration["CardTemplateWithoutStampPath"])));
            services.AddSingleton(new Settings(Path.Combine(Environment.ContentRootPath, "daylimit.txt")));
            // Configuring session
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
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
            // Configuring NLog
            env.ConfigureNLog("nlog.config");
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            app.UseExceptionLogger();

            // Localization
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
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

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

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                SecurityRoutines.CreateAdminUser(userManager, Configuration["admin:login"],
                    Configuration["admin:password"]).Wait();
            }
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Cards}/{action=Create}/{id?}");
            });
        }
    }
}
