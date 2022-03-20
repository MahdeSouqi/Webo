using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebSocketServer.Data;
using WebSocketServer.Middleware;

namespace WebSocketServer
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            var connectionString = "Server=localhost;Port=3306;database=mahde;uid=root;pwd=1jn1kda123sd12;";
            services.AddWebSocketServerConnectionManager();
            services.AddDbContext<MessageContext>(opt =>opt.UseMySql(
               connectionString,serverVersion ));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IMessageRepo, SqlMessageRepo>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseWebSockets();

            app.UseWebSocketServer();
            
            app.Run(async context =>
            {
                
                await context.Response.WriteAsync("Hello from 3rd (terminal) Request Delegate");
            });
        }
    }
}
