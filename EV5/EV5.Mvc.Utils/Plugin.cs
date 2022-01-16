using EV5.Mvc.Embedded;
using EV5.Mvc.Plugin;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EV5.Mvc.DevTools
{
    public class Plugin : IEmbeddedPlugin
    {
        public Assembly WebPartsAssembly => GetType().Assembly;

        public IFileProvider FileProvider => new EV5EmbeddedFileProvider(typeof(Plugin).Assembly, "EV5.MVC-");

        public bool InsertOwnEmbeddedViewEngine => false;

        public string OwnEmbeddedViewEnginePrefix => default;
    }
}
