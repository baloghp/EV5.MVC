using EV5.Mvc.HtmlAgility;
using EV5.Mvc.MEF;
using EV5.Mvc.Services;
using EV5.Mvc.ViewEngine.Providers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EV5.Mvc.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceProvider ServiceProvider = null;


        public static IServiceCollection AddEV5DefaultServices(this IServiceCollection services)
        {

           
            services.AddSingleton<IDocumentHelperFactory>(new HADocumentHelperFactory());
            services.AddTransient<IMarkupProvider, EmbeddedMarkupProvider>();
            services.AddSingleton<IViewClassProvider>(new MEFViewClassProvider());

            ServiceProvider = services.BuildServiceProvider();

            return services;
        }


        public static IMarkupProvider MarkupProvider { get => ServiceProvider.GetService<IMarkupProvider>(); }
        public static IViewClassProvider ViewClassProvider { get => ServiceProvider.GetRequiredService<IViewClassProvider>(); }

        public static IHtmlHelper HtmlHelper { get => ServiceProvider.GetService<IHtmlHelper>(); }

        public static IActionDescriptorCollectionProvider ActionDescriptorCollectionProvider { get => ServiceProvider.GetService<IActionDescriptorCollectionProvider>(); }
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
