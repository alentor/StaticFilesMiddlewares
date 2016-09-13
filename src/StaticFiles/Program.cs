using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace StaticFiles {
    public class Program {
        public static void Main (string[] args) {
            var host = new WebHostBuilder()
            .UseKestrel()
             .UseContentRoot (Directory.GetCurrentDirectory())
             .UseUrls ("http://localhost:8080")
             .UseIISIntegration()
             //.UseStartup (typeof (Program).GetTypeInfo().Assembly.GetName().Name)
             .UseStartup <Startup>()
             .Build();
            host.Run();
        }
    }
}