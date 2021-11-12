//using EV5.Mvc.Extensions;
using EV5.Mvc;
using EV5.Mvc.Embedded;
using EV5.Mvc.Extensions;
using EV5.Mvc.MEF;
using EV5.Samples.Embedded;
using EV5.Samples.ViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace EV5TestWebApp
{
    public class Startup
    {
        IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddRazorPages()
                .AddViewOptions(o => o.ViewEngines.Insert(0, new EmbeddedViewEngine("eve-")))
                ;
           
            //plugins
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(SamplesEmbeddedPlugin).Assembly));
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(SamplesViewEnginePlugin).Assembly));
            //mark the composite providers preferably not one by one, but together at once
            var webOriginalProvider = _env.WebRootFileProvider;
            var samplesembeddedProvider = new EV5EmbeddedFileProvider(typeof(SamplesEmbeddedPlugin).Assembly, "EV5.Samples-");
            var samplesViewengineProvider = new EV5EmbeddedFileProvider(typeof(SamplesViewEnginePlugin).Assembly, "EV5.VE-");
            var webCompositeProvider = new CompositeFileProvider(webOriginalProvider, samplesembeddedProvider, samplesViewengineProvider);
            _env.WebRootFileProvider= webCompositeProvider;

            var contentOriginalProvider = _env.ContentRootFileProvider;
            var contentCompositeProvider = new CompositeFileProvider(contentOriginalProvider, samplesembeddedProvider, samplesViewengineProvider);
            _env.ContentRootFileProvider = contentCompositeProvider;

            //all the changes needed to do once we make plugins
            //register EV5 services
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(EmbeddedViewEngine).Assembly));
            services.AddSingleton<IFileProvider>(_env.WebRootFileProvider);
            services.AddEV5DefaultServices();
            services.AddEV5CompositionServices(new DirCompositionHostFactory(AppDomain.CurrentDomain.BaseDirectory, "EV5*.dll"));

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

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
            Task.Run(async () => await ElectronNET.API.Electron.WindowManager.CreateWindowAsync());
        }
    }
}
