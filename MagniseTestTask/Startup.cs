using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.OpenApi.Models;
using MagniseTestTaskFintacharts.Web;
using Microsoft.EntityFrameworkCore;
using MagniseTestTaskFintacharts.Database;

namespace MagniseTestTaskFintacharts
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
            services.AddControllers();
            services.AddSingleton<Authentication>();
            services.AddSingleton<GetHistoricalData>();
            services.AddSingleton<GetInstruments>();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddDbContext<ApplicationContext>(options =>
                            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddHttpClient("Fintacharts", httpClient =>
            {
                httpClient.BaseAddress = new Uri(Configuration["Web:URI"]);
            });

            services.AddSwaggerGen();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FintachartsApi", Version = "v1" });
            });

           
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
               app.UseDeveloperExceptionPage();
            

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fintacharts API V1"));
            
        }
    }
}
