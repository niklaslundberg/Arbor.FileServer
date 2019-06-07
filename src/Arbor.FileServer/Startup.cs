using System;
using System.IO;
using Arbor.FileServer.Hashing;
using Arbor.KVConfiguration.Core;
using Arbor.KVConfiguration.Microsoft.Extensions.Configuration.Urns;
using Arbor.KVConfiguration.Urns;
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

            MultiSourceKeyValueConfiguration multiSourceKeyValueConfiguration = KeyValueConfigurationManager
                .Add(new EnvironmentVariableKeyValueConfigurationSource())
                .Add(new KeyValueConfigurationAdapter(Configuration))
                .Build();

            var fileServerSettings = multiSourceKeyValueConfiguration.GetInstance<FileServerSettings>();

            if (!Directory.Exists(fileServerSettings.BasePath))
            {
                throw new InvalidOperationException($"The base path '{fileServerSettings.BasePath}' does not exist");
            }

            services.AddSingleton(fileServerSettings);

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

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(hashWatcherSettings.BasePath),
                ServeUnknownFileTypes = true
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
