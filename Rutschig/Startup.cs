using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rutschig.Models;

namespace Rutschig
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<RutschigContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                    pg =>
                    {
                        pg.SetPostgresVersion(6, 0);
                        pg.UseNodaTime();
                    }));

            services.AddSingleton<AppConfig>();

            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            loggerFactory.AddFile(Path.Combine(Path.GetFullPath("./Logs"), "info.log"));
            loggerFactory.AddFile(Path.Combine(Path.GetFullPath("./Logs"), "error.log"), LogLevel.Error);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    "robots",
                    "robots.txt",
                    new {controller = "App", Action = "Robots"});

                endpoints.MapControllerRoute(
                    "sitemap",
                    "sitemap.xml",
                    new {controller = "App", Action = "SiteMap"});

                endpoints.MapControllerRoute(
                    "redirect",
                    "{*alias}",
                    new {controller = "Forward", Action = "Redir"});
            });
        }
    }
}