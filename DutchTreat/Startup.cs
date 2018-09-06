using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DutchTreat
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // add the identity service
            services.AddIdentity<StoreUser, IdentityRole>(cfg => 
            {
                cfg.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<DutchContext>();

            // add support for types of authentication
            services.AddAuthentication()
                .AddCookie()
                // configure JWT support
                .AddJwtBearer(cfg => 
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = _config["Tokens:Issuer"],
                        ValidAudience = _config["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
                    };
                });

            // add database service
            services.AddDbContext<DutchContext>(cfg =>
            {
                // takes in the connection string
                cfg.UseSqlServer(_config.GetConnectionString("DutchConnectionString"));
            });

            // add support for automapper
            services.AddAutoMapper();

            // add a seeder for the database
            services.AddTransient<DutchSeeder>();

            // add repository service and use DutchRepository's implementation
            services.AddScoped<IDutchRepository, DutchRepository>();

            services.AddTransient<IMailService, NullMailService>();
            // Support for real mail service

            // incredibly relevant with today's APIs
            // set compatibility to latest version for MVC 6 features
            services.AddMvc()
                .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
                //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1); // inject MVC
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // set up the order of middleware, ie when a request comes in run some code for me
            // order matters here
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles(); // allows usage of the wwwroot folder static files

            // put before adding MVC
            app.UseAuthentication(); // use authentication and configuration services

            app.UseNodeModules(env); // OdeToCode NuGet pkg to use this

            // leave this at the bottom
            app.UseMvc(cfg => 
            {
                // configure initial routing of MVC
                cfg.MapRoute("Default", 
                    "/{controller}/{action}/{id?}", 
                    new { controller = "App", Action = "Index" });
            });
        }
    }
}
