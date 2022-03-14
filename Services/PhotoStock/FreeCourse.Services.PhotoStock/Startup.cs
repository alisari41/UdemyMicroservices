using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace FreeCourse.Services.PhotoStock
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //PhotoStockApi 'ý koruma altýna alýyorum
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //bu microservice e tokenýn kimin daðýttýðýný haber vericem
                options.Authority = Configuration["IdentityServerUrl"]; //Tokený alýyoruz
                options.Audience = "photo_stock_catalog";// Gelen tokenýn aud parametresi içersinde varmý diye bakýyorum istek yapabilmesi için izin 
                options.RequireHttpsMetadata = false;
            });
            // bütün Controllerlara koruma/izin þartý ver
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter());//Artýk bütün Controllerlara izinlerý authorize larý verdim
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.PhotoStock", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.PhotoStock v1"));
            }

            //static dosyalarýmýzý dýþ dünyaya açmayý saðlar. artýk wwwroot içerisindeki photos dýþ dünyaya açabilirim
            app.UseStaticFiles();

            app.UseRouting();


            // Koruma altýna alýndý
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
