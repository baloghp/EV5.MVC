using EV5.Mvc.Plugin;
using EV5.Mvc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.MEF
{
    public class EmbeddedPluginDefinitionProvider : IEmbeddedPluginDefinitionProvider
    {
        public List<Lazy<IEmbeddedPlugin>> GetEmbeddedPluginList()
        {
            return null;
        }
    }
}
