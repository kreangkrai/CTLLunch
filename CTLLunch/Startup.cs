﻿using CTLLunch.Interface;
using CTLLunch.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTLLunch
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddTransient<IEmployee, EmployeeService>();
            services.AddTransient<IMenu, MenuService>();
            services.AddTransient<IShop, ShopService>();
            services.AddTransient<IReserve, ReserveService>();
            services.AddTransient<IPlanCloseShop, PlanCloseShopService>();
            services.AddTransient<IPlanOutOfIngredients, PlanOutOfIngredientsService>();
            services.AddTransient<ICategory, CategoryService>();
            services.AddTransient<IGroup, GroupService>();
            services.AddTransient<IIngredients, IngredientsService>();
            services.AddTransient<ITransaction, TransactionService>();
            services.AddTransient<IAuthen ,AuthenService>();
            services.AddTransient<ITopup, TopupService>();
            services.AddTransient<IConnectAPI, ConnectAPIService>();
            services.AddTransient<IMail, MailService>();

            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(120);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            RotativaConfiguration.Setup(env);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Index}/{id?}");
            });
        }
    }
}
