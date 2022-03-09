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
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using Microsoft.Extensions.Options;

namespace FreeCourse.Services.Catalog
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
            //Olu�turdu�um service kar��la�t���nda ne yapaca��n� belirtiyorum
            services.AddScoped<ICategoryService, CategoryService>();



            //AutoMapper ekleme
            services.AddAutoMapper(typeof(Startup));

            services.AddControllers();


            // DatabaseSettings ekleme i�lemi //appsettings i�ersindeki verilerimi nesneye d�n��t�r�yorum. Apsettings okuma i�lemi
            services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));

            //Ioptions biz interface �zerinden dolu gelsin oradan okuyabileyim
            services.AddSingleton<IDatabaseSettings>(sp =>
            {
                //GetRequiredService ilgili servisi bulamazsa hata f�rlat�r.
                return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Catalog", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Catalog v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
