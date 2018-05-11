using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using BachelorMVC.Persistence;
using BachelorMVC.Services;

namespace BachelorMVC
{
    public class Startup
    {


        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddTransient<IbrukerService, BrukerService>();



            //services.AddDbContext<BachelorDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default"))); //passord:"Dokumentpartner01!"
            services.AddDbContext<BachelorDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
            // Add framework services.

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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




            ////////////////////////////////////////////////////////////

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true
            });

            var options = new OpenIdConnectOptions()
            {

                
                
                AuthenticationScheme = "Oidc", // callback will be on /signin-oidc
                SignInScheme = "Cookies",
                ResponseType = "code",
                Authority = "https://document.eu.auth0.com", // For testing: "https://acme-corp.grean.id/"
                ClientId = "mP7idivyJncZ0XWvo3zC9K65Fr843FZk", // For testing: "0m4bGC+LO7QSBk7zf4d2Uhhlq48IRHbUC/D5yM4EROU="
                ClientSecret = "tpXUWzv0ylsN8b3Af7sTpeMQl62EnQwsO6RvuuYdCtvgl7FJiscyjau3F7-LsDkG", // For testing: "0m4bGC+LO7QSBk7zf4d2Uhhlq48IRHbUC/D5yM4EROU="
                
                
        };
            

            options.RequireHttpsMetadata = false;
            
            

            // This may be modified to get the choice of authentication method from
            // some other source, e.g. a dropdown in the UI
            // EasyID relies on this, but not needed for most OIDC identity proivders, such as Google, etc.
            options.Events = new OpenIdConnectEvents()
            {
                OnRedirectToIdentityProvider = context => {
                    
                    context.ProtocolMessage.AcrValues = "tenant:default";
                    return Task.FromResult(0);
                },

                 OnMessageReceived = context =>
                 {
                     if (context.ProtocolMessage.Error == "unauthorized")
                     {
                         context.HandleResponse();
                         context.Response.Redirect("/?unauth=1");
                     }

                     return Task.FromResult(0);
                 },
            };

            // Wire in OIDC middelware
            app.UseOpenIdConnectAuthentication(options);



            ////////////////////////////////////////////////////////////

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                
                /*routes.MapRoute(
                    name: "OpprettCaseOgSendEpost",
                    template: "{controller=HomeController}/{action=OpprettCaseOgSendEpost}",
                    defaults: new { controller = "HomeController", action = "OpprettCaseOgSendEpost" }
                    );*/
            });
        }
    }

}
