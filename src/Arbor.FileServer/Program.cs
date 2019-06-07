using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Arbor.FileServer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                IWebHost webHost = CreateWebHostBuilder(args).Build();

                webHost.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
