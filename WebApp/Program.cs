using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApp
{
    public class Program
    {

        public static IHost WebHost;
        public static void Main(string[] args)
        {
            WebHost = CreateHostBuilder(args).Build();
            
            try
            {
                WebHost.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("Shutdown from code! " + e);
            }
           

            Console.Write("Play your game!");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}