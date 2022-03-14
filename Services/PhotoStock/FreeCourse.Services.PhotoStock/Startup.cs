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
            //PhotoStockApi '� koruma alt�na al�yorum
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //bu microservice e token�n kimin da��tt���n� haber vericem
                options.Authority = Configuration["IdentityServerUrl"]; //Token� al�yoruz
                options.Audience = "photo_stock_catalog";// Gelen token�n aud parametresi i�ersinde varm� diye bak�yorum istek yapabilmesi i�in izin 
                options.RequireHttpsMetadata = false;
            });
            // b�t�n Controllerlara koruma/izin �art� ver
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter());//Art�k b�t�n Controllerlara izinler� authorize lar� verdim
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

            //static dosyalar�m�z� d�� d�nyaya a�may� sa�lar. art�k wwwroot i�erisindeki photos d�� d�nyaya a�abilirim
            app.UseStaticFiles();

            app.UseRouting();


            // Koruma alt�na al�nd�
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
