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

namespace EV5.Samples.Embedded
{
    
    public class SamplesEmbeddedPlugin : IEmbeddedPlugin
    {
        public IFileProvider FileProvider => new EV5EmbeddedFileProvider(typeof(SamplesEmbeddedPlugin).Assembly, "EV5.Samples-");

        public bool InsertOwnEmbeddedViewEngine => false;

        public string OwnEmbeddedViewEnginePrefix => default;

        public Assembly WebPartsAssembly => this.GetType().Assembly;
    }
}
