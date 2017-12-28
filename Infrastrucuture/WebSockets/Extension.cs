

using Microsoft.AspNetCore.Http;
using NotificationBackend.Infrastrucuture.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace NotificationBackend.Infrastrucuture.WebSockets
{
    public static class Extensions
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                             PathString path,
                                                             WebSocketHandler handler) => app.Map(path, (_app) => _app.UseMiddleware<AppWebSocketMidleware>(handler));
    
    public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            foreach(var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if(type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}