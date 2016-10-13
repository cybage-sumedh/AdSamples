using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreAuthCookieTest
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // We need to configure data protection to use a specific key directory
            // we can share between applications.
            //
            // We also need a common protector purpose, as different purposes are
            // automatically isolated from one another.
            //
            // Finally we need to wire up a common ticket formatter.

            // Normally you'd just have a hard coded, or configuration based path,
            // but for this demo we're going to share a directory in the solution directory,
            // so we have to do some jiggery-pokery to figure it out.
            string contentRoot = env.ContentRootPath;
            string keyRingPath = Path.GetFullPath(Path.Combine(contentRoot, "..", "keys"));

            //string keyRingPath = @"E:\Common\SSO\AdfsSample\keys\sample";

            // Now we create a data protector, with a fixed purpose and sub-purpose used in key derivation.
            var protectionProvider = DataProtectionProvider.Create(new DirectoryInfo(keyRingPath));
            var dataProtector = protectionProvider.CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                "Cookie",
                "v2");
            // And finally create a new auth ticket formatter using the data protector.
            var ticketFormat = new TicketDataFormat(dataProtector);

            // Now configure the cookie options to have the same cookie name, and use
            // the common format.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookie",
                CookieName = ".AspNet.SharedCookie",
                TicketDataFormat = ticketFormat
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
