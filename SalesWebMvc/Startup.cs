﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Services;

namespace SalesWebMvc
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
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Para o método funcionar direito, temos que incluir a dependencia do MySQL (Pomelo.EntityFrameworkCore.MySql)
            services.AddDbContext<SalesWebMvcContext>(options =>
            // Para fazer a conexão com o DB, temos que usar a class que estádentro dá pasta Data (SalesWebMvcContext)
                    options.UseMySql(Configuration.GetConnectionString("SalesWebMvcContext"), builder =>
                    // O nome do nossa Assembly é o mesmo nome do nosso projeto. (SalesWebMvc)
                    builder.MigrationsAssembly("SalesWebMvc")));

            // Registrando o Serviço do SeedingService.
            services.AddScoped<SeedingService>();

            // Registrando o Serviço do Service
            services.AddScoped<SellerService>();

            // Registrando o Serviço do Departamento
            services.AddScoped<DepartmentService>();

        }

        /*
         * Esse método aceita novos parâmetros desde que ele esteja registrado
         * no ConfigureService. Assim ele já resolve uma estância da aplicação.
        */
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SeedingService seedingService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Estando no estado de desenvolvimento, o meu banco será populado.
                seedingService.Seed();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
