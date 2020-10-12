using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Splitio.Services.Client.Classes;
using Splitio.Services.Client.Interfaces;

namespace FeatureFlagsSplit
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("MyDemoInMemoryDb"));

            services.AddSingleton<ISplitFactory, SplitFactory>(s => new SplitFactory(_configuration["Split:ApiKey"], new ConfigurationOptions()
            {
                StreamingEnabled = false,
                FeaturesRefreshRate = 1,
                SegmentsRefreshRate = 1
            }));

            services.AddSwaggerGen(action =>
            {
                action.SwaggerDoc("v1", new OpenApiInfo {Title = _env.ApplicationName + "API", Version = "v1"});
                action.CustomSchemaIds((type) => type.FullName);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", env.ApplicationName));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}


