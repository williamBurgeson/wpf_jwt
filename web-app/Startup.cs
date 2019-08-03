using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web_app
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public AppSettings AppSettings => Configuration.GetSection("AppSettings").Get<AppSettings>();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SecurityDataContext>(x => x.UseInMemoryDatabase("SecurityDb"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.Configure<List<UserEntity>>(appSettingsSection.GetSection("Users"));

            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<ISecurityService>();
                        var username = context.Principal.Identity.Name;
                        var user = userService.GetByUsername(username);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services
            services.AddScoped<ISecurityService, SecurityService>();

            PopulateDatabase(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        private void PopulateDatabase(IServiceCollection services)
        {
            var dbContext = services.BuildServiceProvider()
                .GetRequiredService<SecurityDataContext>();

            var prov = ((ConfigurationRoot)this.Configuration)
                .Providers
                .OfType<JsonConfigurationProvider>()
                .FirstOrDefault(x => x.TryGet("AppSettings:Secret", out _));

            int index = 0;
            var users = new List<UserEntity>();

            while (prov.TryGet($"AppSettings:Users:{index}:Username", out string username))
            {
                prov.TryGet($"AppSettings:Users:{index}:PasswordHash", out string passwordHashB64);
                prov.TryGet($"AppSettings:Users:{index}:PasswordHash", out string passwordSaltB64);
                prov.TryGet($"AppSettings:Users:{index}:LastUpdated", out string lastUpdatedStr);
                var user = new UserEntity
                {
                    Username = username,
                    PasswordHash = Convert.FromBase64String(passwordHashB64),
                    PasswordSalt = Convert.FromBase64String(passwordSaltB64),
                    LastUpdated = DateTime.Parse(lastUpdatedStr)
                };
                users.Add(user);
                index++;
            }

            if (users.GroupBy(x => x.Username.ToLower()).Where(x => x.Count() > 1).Any())
                throw new InvalidOperationException(
                    "Usernames in the app settings file must be unique when compared case-insensitively");

            dbContext.Users.AddRange(users);

            dbContext.SaveChanges();
        }
    }
}
