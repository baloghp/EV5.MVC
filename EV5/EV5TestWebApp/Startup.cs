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
            services.UseEmbeddedPlugins(new DirCompositionHostFactory(AppDomain.CurrentDomain.BaseDirectory, "EV5*.dll"));


            //Always call this last. The internal EV5 ServiceProvider will be registered at this point,
            //It will only be able to find services registered so far
            services.RegisterEV5ServiceProvider();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostEnvironment henv)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Error");
                //The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetco        app.UseHsts();
            }

            // this call will configure the embedded filesystems used by the plugins
            IHostEnvironment hostEnvironment = env.CreateEV5FileProvider(env.WebRootFileProvider, out IFileProvider EV5FileProvider);
            env.WebRootFileProvider = EV5FileProvider;
            env.ContentRootFileProvider = EV5FileProvider;
            henv.ContentRootFileProvider = EV5FileProvider;



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


            ElectronBootstrap();

            // Open the Electron-Window here
            //Task.Run(async () => {
            //    var window = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync();
            //    window.OnReadyToShow += () => window.Show();
            //});
        }

        public async void ElectronBootstrap()
        {
            if (HybridSupport.IsElectronActive)
            {
                SetMenu();
                var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
                {
                    Width = 1152,
                    Height = 940,
                    Show = false
                });

                await browserWindow.WebContents.Session.ClearCacheAsync();

                browserWindow.OnReadyToShow += () => browserWindow.Show();
                browserWindow.SetTitle("EV5 Demo");
            }
        }

        private void SetMenu()
        {
            if (HybridSupport.IsElectronActive)
            {
                var menu = new MenuItem[] {
                //new MenuItem { Label = "Edit", Type = MenuType.submenu, Submenu = new MenuItem[] {
                //    new MenuItem { Label = "Undo", Accelerator = "CmdOrCtrl+Z", Role = MenuRole.undo },
                //    new MenuItem { Label = "Redo", Accelerator = "Shift+CmdOrCtrl+Z", Role = MenuRole.redo },
                //    new MenuItem { Type = MenuType.separator },
                //    new MenuItem { Label = "Cut", Accelerator = "CmdOrCtrl+X", Role = MenuRole.cut },
                //    new MenuItem { Label = "Copy", Accelerator = "CmdOrCtrl+C", Role = MenuRole.copy },
                //    new MenuItem { Label = "Paste", Accelerator = "CmdOrCtrl+V", Role = MenuRole.paste },
                //    new MenuItem { Label = "Select All", Accelerator = "CmdOrCtrl+A", Role = MenuRole.selectall }
                //}
                //},
                new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = new MenuItem[] {
                    new MenuItem
                    {
                        Label = "Reload",
                        Accelerator = "CmdOrCtrl+R",
                        Click = () =>
                        {
                            // on reload, start fresh and close any old
                            // open secondary windows
                            var mainWindowId = Electron.WindowManager.BrowserWindows.ToList().First().Id;
                            Electron.WindowManager.BrowserWindows.ToList().ForEach(browserWindow => {
                                if(browserWindow.Id != mainWindowId)
                                {
                                    browserWindow.Close();
                                }
                                else
                                {
                                    browserWindow.Reload();
                                }
                            });
                        }
                    },
                    new MenuItem
                    {
                        Label = "Toggle Full Screen",
                        Accelerator = "CmdOrCtrl+F",
                        Click = async () =>
                        {
                            bool isFullScreen = await Electron.WindowManager.BrowserWindows.First().IsFullScreenAsync();
                            Electron.WindowManager.BrowserWindows.First().SetFullScreen(!isFullScreen);
                        }
                    },
                    new MenuItem
                    {
                        Label = "Open Developer Tools",
                        Accelerator = "CmdOrCtrl+I",
                        Click = () => Electron.WindowManager.BrowserWindows.First().WebContents.OpenDevTools()
                    },
                    new MenuItem
                    {
                        Type = MenuType.separator
                    }
                    //,
                    //new MenuItem
                    //{
                    //    Label = "App Menu Demo",
                    //    Click = async () => {
                    //        var options = new MessageBoxOptions("This demo is for the Menu section, showing how to create a clickable menu item in the application menu.");
                    //        options.Type = MessageBoxType.info;
                    //        options.Title = "Application Menu Demo";
                    //        await Electron.Dialog.ShowMessageBoxAsync(options);
                    //    }
                    //}
                }
                }
                //,
                //new MenuItem { Label = "Window", Role = MenuRole.window, Type = MenuType.submenu, Submenu = new MenuItem[] {
                //     new MenuItem { Label = "Minimize", Accelerator = "CmdOrCtrl+M", Role = MenuRole.minimize },
                //     new MenuItem { Label = "Close", Accelerator = "CmdOrCtrl+W", Role = MenuRole.close }
                //}
                //},
                //new MenuItem { Label = "Help", Role = MenuRole.help, Type = MenuType.submenu, Submenu = new MenuItem[] {
                //    new MenuItem
                //    {
                //        Label = "Learn M",
                //        Click = async () => await Electron.Shell.OpenExternalAsync("https://github.com/ElectronNET")
                //    }
                //}
                //}
            };

                Electron.Menu.SetApplicationMenu(menu);

                //CreateContextMenu();
            }
        }
    }


}
