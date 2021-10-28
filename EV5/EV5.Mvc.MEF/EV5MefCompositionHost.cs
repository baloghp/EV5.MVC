using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.MEF
{
    public  static class EV5MefCompositionHost
    {
        public static IServiceProvider ServiceProvider = null;

        public static CompositionHost CompositionHost;

        public static IServiceCollection AddEV5MefServices(this IServiceCollection services,  ICompositionHostFactory compositionHostFactory)
        {
            ServiceProvider = services.BuildServiceProvider();

            services.AddSingleton<ICompositionHostFactory>(compositionHostFactory);
            CompositionHost = compositionHostFactory.CreateCompositionHost();
            //services.AddSingleton<CompositionHost>(CompositionHost);
            return services;
        }
    }
}
