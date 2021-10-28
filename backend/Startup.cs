using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace backend
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
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "backend", Version = "v1" }); });

            ConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");
            services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);

            FlushRedisDb(redisConnectionMultiplexer);
            PlzData.ImportToRedis(redisConnectionMultiplexer);
        }

        private static void FlushRedisDb(ConnectionMultiplexer redisConnectionMultiplexer)
        {
            IServer server = redisConnectionMultiplexer.GetServer(redisConnectionMultiplexer.GetEndPoints().First());
            server.FlushDatabase();
            Debug.Assert(!server.Keys().Any());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
    }

}
