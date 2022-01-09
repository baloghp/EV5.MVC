//using EV5.Mvc.Extensions;
using ElectronNET.API;
using ElectronNET.API.Entities;
using EV5.Mvc;
using EV5.Mvc.Embedded;
using EV5.Mvc.Extensions;
using EV5.Mvc.MEF;
using EV5.Samples.Embedded;
using EV5.Samples.ViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace EV5TestWebApp
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
            //register the EmbeddedViewEngine, with a prefix as the first ViewEngine.
            //If a viewname will mathc to multiple across viewengines this one will win.
            //Prefix ensures that any viewname without that prefix will be quickly passed forward without further examination
            //when using prefixes always make sure that the views in the plugins are aware of them
            services.AddRazorPages()
                .AddViewOptions(o => o.ViewEngines.Insert(0, new EmbeddedViewEngine("eve-")));
            
            //this call sets up the default EV5 Services
            services.AddEV5DefaultServices();


            //This method will discover all exported IEmbeddedPlugins in the provided CompositionHostFactory
            //It will then use the information in these objects to set up the web components and.
            services.UseEmbeddedPlugins(new DirCompositionHostFactory(AppDomain.CurrentDomain.BaseDirectory, "EV5*.dll")
                                        ,registerPluginViewEngines:true);

            
            //Always call this last. The internal EV5 ServiceProvider will be registered at this point,
            //It will only be able to find services registered so far
            services.RegisterEV5ServiceProvider();
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Error");
                //The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // this call will configure the embedded filesystems used by the plugins
            env.ConfigureEmbeddedPlugins();


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

            // Open the Electron-Window here
            ElectronBootstrap();
        }

        public async void ElectronBootstrap()
        {
            if (HybridSupport.IsElectronActive)
            {

                var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
                {
                    Width = 1152,
                    Height = 940,
                    Show = false
                });

                await browserWindow.WebContents.Session.ClearCacheAsync();

                browserWindow.OnReadyToShow += () => browserWindow.Show();
                browserWindow.SetTitle("Electron.NET API Demos");
            }
        }
    }

   
}
