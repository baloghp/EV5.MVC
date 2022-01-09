using EV5.Mvc.Embedded;
using EV5.Mvc.Plugin;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Shop
{
    public class Plugin : IEmbeddedPlugin
    {
        public Assembly WebPartsAssembly => GetType().Assembly;

        public IFileProvider FileProvider => new EV5EmbeddedFileProvider(typeof(Plugin).Assembly, "EV5.SHOP-");

        public bool InsertOwnEmbeddedViewEngine => true;

        public string OwnEmbeddedViewEnginePrefix => "ev5.shop-";
    }
}
