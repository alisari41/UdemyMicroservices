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
using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

namespace FreeCourse.Services.Basket
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
            //laz�m 
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            //BasketAPI '� koruma alt�na al�yorum
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //bu microservice e token�n kimin da��tt���n� haber vericem
                options.Authority = Configuration["IdentityServerUrl"]; //Token� al�yoruz
                options.Audience = "resource_catalog";// Gelen token�n aud parametresi i�ersinde varm� diye bak�yorum istek yapabilmesi i�in izin 
                options.RequireHttpsMetadata = false;
            });
            //SharedIdentityService i�in 
            services.AddHttpContextAccessor();
            // Bu interface ile kar��la�t���n zaman git SharedIdentityService den bana nesne �rne�i al
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            //Bu servisleri kullanmak i�in eklemek gerekir
            services.AddScoped<IBasketService, BasketService>();


            //RedisSettings Host Port bilgilerine eri�mek i�in              appsettings.json i�ersindeki bilgileri al�cak
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));

            //Direk aya�a kalkt���nda ba�lant� kurulmas�n� istiyorum.
            services.AddSingleton<RedisService>(sp =>
            {
                // appsettings.json daki de�erleri okucam
                var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;

                var redis = new RedisService(redisSettings.Host, redisSettings.Port);

                redis.Connect();

                return redis;
            });

            //b�t�n Controllerlara koruma/izin �art� ver
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Basket", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Basket v1"));
            }

            app.UseRouting();


            //yetkiyi ekle
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
