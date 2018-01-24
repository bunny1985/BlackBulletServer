using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NotificationBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>


            

            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://::8080" )  
                
                // .UseKestrel(options => 
                // {
                //     options.Listen(IPAddress.Any , 8080 , listenoptions => {
                //         listenoptions.UseHttps("cert.cer");
                //     });
                // })
                .UseStartup<Startup>()
                .Build();
    }
}
