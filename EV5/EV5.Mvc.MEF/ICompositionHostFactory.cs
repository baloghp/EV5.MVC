using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.MEF
{
    public interface ICompositionHostFactory
    {
        public CompositionHost CreateCompositionHost();
    }
}
