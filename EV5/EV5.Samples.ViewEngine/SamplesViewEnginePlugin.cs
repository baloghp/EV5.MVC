using EV5.Mvc.Embedded;
using EV5.Mvc.Plugin;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Samples.ViewEngine
{
    
    public class SamplesViewEnginePlugin : IEmbeddedPlugin
    {
        public IFileProvider FileProvider => new EV5EmbeddedFileProvider(typeof(SamplesViewEnginePlugin).Assembly, "EV5.VE-");

        public bool InsertOwnEmbeddedViewEngine => true;

        public string OwnEmbeddedViewEnginePrefix => "EV5";

        public Assembly WebPartsAssembly => this.GetType().Assembly;

    }
}
