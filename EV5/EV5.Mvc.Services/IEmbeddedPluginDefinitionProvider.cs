using EV5.Mvc.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.Services
{
    public interface IEmbeddedPluginDefinitionProvider
    {
        public abstract List<Lazy<IEmbeddedPlugin>> GetEmbeddedPluginList();
    }
}
