using EV5.Mvc.Plugin;
using EV5.Mvc.ViewEngine;
using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.MEF
{
    public class DirCompositionHostFactory : ICompositionHostFactory
    {
        string path;
        string searchPattern;
        SearchOption searchOption;

        public DirCompositionHostFactory(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            this.path = path;
            this.searchPattern = searchPattern;
            this.searchOption = searchOption;
        }

        public CompositionHost CreateCompositionHost()
        {
            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IEmbeddedView>().Export<IEmbeddedView>().Shared();
            conventions.ForTypesDerivedFrom<IEmbeddedPlugin>().Export<IEmbeddedPlugin>().Shared();

            //    .ForTypesDerivedFrom<IPlugin>()
            //    .Export<IPlugin>()
            //    .Shared();

            //var assemblies = new[] { typeof(MyPlugin).Assembly };

            //var configuration = new ContainerConfiguration()
            //    .WithAssemblies(assemblies, conventions);

            //using (var container = configuration.CreateContainer())
            //{
            //    var plugins = container.GetExports<IPlugin>();
            //}
            //var path = ;

            var configuration = new ContainerConfiguration()
                .WithAssembliesInPath(path, searchPattern, conventions, searchOption);
            return configuration.CreateContainer();
        }
    }

    public static class ContainerConfigurationExtensions
    {
        public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return WithAssembliesInPath(configuration, path, searchPattern, null, searchOption);
        }

        public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, string searchPattern, AttributedModelProvider conventions, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var assemblies = Directory
                .GetFiles(path, searchPattern, searchOption)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            configuration = configuration.WithAssemblies(assemblies, conventions);

            return configuration;
        }
    }
}
