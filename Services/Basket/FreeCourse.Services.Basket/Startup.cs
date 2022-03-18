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
using System.IdentityModel.Tokens.Jwt;
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
            //lazým 
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            //Mapleme olayýný kaldýrmasýný haberdar edicem yani ben token cliam "sub" olarak beklerken kendi farklý isim veriyor
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            //BasketAPI 'ý koruma altýna alýyorum
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //bu microservice e tokenýn kimin daðýttýðýný haber vericem
                options.Authority = Configuration["IdentityServerUrl"]; //Tokený alýyoruz
                options.Audience = "resource_basket";// Gelen tokenýn aud parametresi içersinde varmý diye bakýyorum istek yapabilmesi için izin 
                options.RequireHttpsMetadata = false;
            });
            //SharedIdentityService için 
            services.AddHttpContextAccessor();
            // Bu interface ile karþýlaþtýðýn zaman git SharedIdentityService den bana nesne örneði al
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();
            //Bu servisleri kullanmak için eklemek gerekir
            services.AddScoped<IBasketService, BasketService>();


            //RedisSettings Host Port bilgilerine eriþmek için              appsettings.json içersindeki bilgileri alýcak
            services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));

            //Direk ayaða kalktýðýnda baðlantý kurulmasýný istiyorum.
            services.AddSingleton<RedisService>(sp =>
            {
                // appsettings.json daki deðerleri okucam
                var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;

                var redis = new RedisService(redisSettings.Host, redisSettings.Port);

                redis.Connect();

                return redis;
            });

            //bütün Controllerlara koruma/izin þartý ver
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
