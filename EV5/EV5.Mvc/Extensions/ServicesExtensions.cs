using EV5.Mvc.HtmlAgility;
using EV5.Mvc.Services;
using EV5.Mvc.ViewEngine.Providers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
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
            ServiceProvider = services.BuildServiceProvider();

            services.AddSingleton<IDocumentHelperFactory>(new HADocumentHelperFactory());
            services.AddTransient<IMarkupProvider, EmbeddedMarkupProvider>();
            services.AddTransient<IViewClassProvider, >();

            return services;
        }

        public static IHtmlGenerator HtmlGenerator { get => ServiceProvider.GetService<IHtmlGenerator>();  }
        public static IModelMetadataProvider ModelMetadataProvider { get => ServiceProvider.GetService<IModelMetadataProvider>(); }
        public static ICompositeViewEngine CompositeViewEngine { get => ServiceProvider.GetService<ICompositeViewEngine>(); }
        public static IViewBufferScope ViewBufferScope { get => ServiceProvider.GetService<IViewBufferScope>(); }
        public static HtmlEncoder HtmlEncoder { get => ServiceProvider.GetService<HtmlEncoder>(); }
        public static UrlEncoder UrlEncoder { get => ServiceProvider.GetService<UrlEncoder>(); }
         public static IMarkupProvider MarkupProvider { get => ServiceProvider.GetService<IMarkupProvider>(); }
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
