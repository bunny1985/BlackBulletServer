using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationBackend.Infrastrucuture.Database;
using Swashbuckle.AspNetCore.Swagger;
using NotificationBackend.Infrastrucuture.WebSockets;
using System.IO;
using Serilog;
using Datalust.SerilogMiddlewareExample.Diagnostics;

namespace NotificationBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
      .Enrich.FromLogContext()
        
      .WriteTo.RollingFile("log-{Date}.txt")
      .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
            services.AddDbContext<MyDbContext>(options => options.UseSqlite("Data Source=MvcMovie.db"));
            services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<MyDbContext>()
        .AddDefaultTokenProviders();
            services.AddWebSocketManager();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider , ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<SerilogMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseMvc();
            app.UseWebSockets();
            app.MapWebSocketManager("/notificationsSocket", serviceProvider.GetService<NotificationSocketHandler>());
        }
    }
}
