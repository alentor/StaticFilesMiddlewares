using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace StaticFiles {
    public class Startup {
        public Startup (IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
            .SetBasePath (env.ContentRootPath)
             .AddJsonFile ("appsettings.json", optional:true, reloadOnChange:true)
             .AddJsonFile ($"appsettings.{env.EnvironmentName}.json", optional:true)
             .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration{get;}
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            // Add framework services.
            services.AddDirectoryBrowser();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole (Configuration.GetSection ("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else {
                app.UseExceptionHandler ("/Home/Error");
            }
            FileExtensionContentTypeProvider fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            // Add new mappings
            fileExtensionContentTypeProvider.Mappings[".myapp"] = "application/x-msdownload";
            fileExtensionContentTypeProvider.Mappings[".htm3"] = "text/html";
            fileExtensionContentTypeProvider.Mappings[".cs"] = "text";
            fileExtensionContentTypeProvider.Mappings[".js"] = "text/html";
            fileExtensionContentTypeProvider.Mappings[".cshtml"] = "text/css";
            fileExtensionContentTypeProvider.Mappings[".image"] = "image/png";
            // Replace an existing mapping
            fileExtensionContentTypeProvider.Mappings[".rtf"] = "application/x-msdownload";
            // Remove MP4 videos.
            fileExtensionContentTypeProvider.Mappings.Remove (".mp4");
            // Use to serve directories and files
            app.UseFileServer (new FileServerOptions() {
                                   FileProvider = new PhysicalFileProvider (Directory.GetCurrentDirectory()),
                                   //FileProvider = new PhysicalFileProvider (Path.Combine (Directory.GetCurrentDirectory(), "MyStaticFiles")),
                                   RequestPath = new PathString ("/root"),
                                   EnableDirectoryBrowsing = true,
                                   EnableDefaultFiles = true,
                                   //StaticFileOptions = {ContentTypeProvider = new FileExtensionContentTypeProvider {Mappings = {new KeyValuePair <string, string> (".cs", "text/css")}},}
                                   StaticFileOptions = {ContentTypeProvider = fileExtensionContentTypeProvider}
                               });
            // Use to serve files ex. http://localhost:8080/root2/views/_viewstart.cshtml 
            app.UseStaticFiles (new StaticFileOptions {
                                    FileProvider = new PhysicalFileProvider (Directory.GetCurrentDirectory()),
                                    ServeUnknownFileTypes = true,
                                    //DefaultContentType = "text/css",
                                    RequestPath = new PathString ("/root2"),
                                    ContentTypeProvider = fileExtensionContentTypeProvider,
                                });
            // Use to serve  directories, default: wwwroot
            //app.UseDirectoryBrowser();
            app.UseStaticFiles();
            app.UseMvc (routes => {routes.MapRoute (name:"default", template:"{controller=Home}/{action=Index}/{id?}");});
        }
    }
}