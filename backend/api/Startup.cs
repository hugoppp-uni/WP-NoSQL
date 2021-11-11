using System.IO;
using backend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace backend
{

    public class Startup
    {
        private readonly Db _dbToUse;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _dbToUse = configuration.GetValue<Db>("Db");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "WPP NoSql API - Alexander Könemann, Hugo Protsch", Version = "v1" });
                options.IncludeXmlComments(Path.Combine("bin","doc.xml"));
            });

            // connect to db lazily
            services.AddSingleton((_) => ConnectionCreator.Mongo());
            services.AddSingleton<IConnectionMultiplexer>((_) => ConnectionCreator.Redis());

            if (_dbToUse == Db.Redis)
                services.AddSingleton<ICityService, RedisCityService>();
            else if (_dbToUse == Db.MongoDB)
                services.AddSingleton<ICityService, MongoCityService>();

            services.AddSingleton<MongoCityService>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Using {} as Database", _dbToUse);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "backend v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private enum Db
        {
            Redis,
            MongoDB,
        }
    }

}
