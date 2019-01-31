using System;
using System.IO;
using Arbor.FileServer.Hashing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Arbor.FileServer
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
            services.AddSingleton<HashWatcher>();
            services.AddHostedService<FileWatcherService>();

            IConfigurationSection fileServer = Configuration.GetSection("file-server");

            if (!SupportedHashAlgorithm.TryParse(fileServer.GetValue<string>("hashAlgorithm"),
                out SupportedHashAlgorithm hashAlgorithm))
            {
                throw new InvalidOperationException("The hash algorithm is not set");
            }

            string basePath = fileServer.GetValue<string>("basePath");
            string baseUrl = fileServer.GetValue<string>("baseUrl");

            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new InvalidOperationException("The base path has not been set");
            }

            if (!Directory.Exists(basePath))
            {
                throw new InvalidOperationException($"The base path '{basePath}' does not exist");
            }

            services.AddSingleton(new FileServerSettings(basePath, hashAlgorithm, baseUrl));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var hashWatcherSettings = app.ApplicationServices.GetService<FileServerSettings>();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(hashWatcherSettings.BasePath),
                ServeUnknownFileTypes = true
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}