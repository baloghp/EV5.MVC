using Microsoft.Extensions.FileProviders;
using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace EV5.Mvc.Plugin
{
    [InheritedExport(typeof(IEmbeddedPlugin))]
    public interface IEmbeddedPlugin
    {
        Assembly WebPartsAssembly { get; }
        IFileProvider FileProvider {get;}

        bool InsertOwnEmbeddedViewEngine { get; }
        string OwnEmbeddedViewEnginePrefix { get; }
    }
}
