using EV5.Mvc.ViewEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.Services
{
    public interface  IViewClassProvider
    {
        public abstract IEmbeddedView GetEmbeddedViewClass(string viewName);
}
}
