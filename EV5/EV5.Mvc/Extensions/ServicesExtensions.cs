using EV5.Mvc.HtmlAgility;
using EV5.Mvc.MEF;
using EV5.Mvc.Plugin;
using EV5.Mvc.Services;
using EV5.Mvc.ViewEngine.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EV5.Mvc.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceProvider ServiceProvider = null;

        public static IMarkupProvider MarkupProvider { get => ServiceProvider.GetService<IMarkupProvider>(); }
        public static IViewClassProvider ViewClassProvider { get => ServiceProvider.GetRequiredService<IViewClassProvider>(); }
        public static IHtmlHelper HtmlHelper { get => ServiceProvider.GetService<IHtmlHelper>(); }
        public static IActionDescriptorCollectionProvider ActionDescriptorCollectionProvider { get => ServiceProvider.GetService<IActionDescriptorCollectionProvider>(); }
        
        public static IServiceCollection AddEV5DefaultServices(this IServiceCollection services)
        {

            services.AddMvcCore().AddApplicationPart(typeof(EmbeddedViewEngine).Assembly);
            services.AddSingleton<IDocumentHelperFactory>(new HADocumentHelperFactory());
            services.AddTransient<IMarkupProvider, EmbeddedMarkupProvider>();
            services.AddSingleton<IViewClassProvider>(new MEFViewClassProvider());

            ServiceProvider = services.BuildServiceProvider();

            return services;
        }

        public static IServiceCollection RegisterEV5ServiceProvider(this IServiceCollection services)
        {
            ServiceProvider = services.BuildServiceProvider();
            return services;
        }

        public static IServiceCollection UseEmbeddedPlugins(this IServiceCollection services,
            ICompositionHostFactory hostFactory,
            Func<IEnumerable<IEmbeddedPlugin>, IEnumerable<IEmbeddedPlugin>> BeforePluginsInitialized = null
            )
        {
            
            services.AddEV5CompositionServices(hostFactory);
            var plugins = EV5MefCompositionHost.CompositionHost.GetExports<IEmbeddedPlugin>();
            if (BeforePluginsInitialized != null)
                plugins = BeforePluginsInitialized(plugins);
            foreach (var p in plugins)
            {
                services.AddMvcCore().AddApplicationPart(p.WebPartsAssembly);
                if (p.InsertOwnEmbeddedViewEngine)
                {
                    services.AddMvc()
                        .AddViewOptions(o => o.ViewEngines
                                              .Insert(0, new EmbeddedViewEngine(p.OwnEmbeddedViewEnginePrefix)));
                }
            }


            ServiceProvider = services.BuildServiceProvider();
            
            return services;
        }

        public static IWebHostEnvironment ConfigureEmbeddedPlugins(this IWebHostEnvironment env,
            Action<IFileProvider> OnCompositeFileProviderPrepared = null,
            Func<IEnumerable<IEmbeddedPlugin>, IEnumerable<IEmbeddedPlugin>> BeforePluginsInitialized = null
            )
        {

            var plugins = EV5MefCompositionHost.CompositionHost.GetExports<IEmbeddedPlugin>();
            if (BeforePluginsInitialized != null)
                plugins = BeforePluginsInitialized(plugins);
            
            var fileproviders = plugins.Select(p => p.FileProvider).Union(new List<IFileProvider> { env.WebRootFileProvider, env.ContentRootFileProvider });
            CompositeFileProvider compositeProvider = new CompositeFileProvider(fileproviders.ToArray());
            if (OnCompositeFileProviderPrepared != null)
                OnCompositeFileProviderPrepared(compositeProvider);
            env.WebRootFileProvider = compositeProvider;
            env.ContentRootFileProvider = compositeProvider;

            return env;
        }
    }

    public static class DocumentHelperFactory
    {
        public static IDocumentHelperFactory Factory = null;

        static DocumentHelperFactory()
        {
            Factory = ServicesExtensions.ServiceProvider.GetService<IDocumentHelperFactory>();
        }

    }
}
