﻿using EV5.Mvc.Embedded;
using EV5.Mvc.Plugin;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EV5.AdminLTE
{
    public class Plugin : IEmbeddedPlugin
    {
        public Assembly WebPartsAssembly => GetType().Assembly;

        public IFileProvider FileProvider => new EV5EmbeddedFileProvider(typeof(Plugin).Assembly, "EV5.ALTE-");

        public bool InsertOwnEmbeddedViewEngine => false;

        public string OwnEmbeddedViewEnginePrefix => throw new NotImplementedException();
    }
}
