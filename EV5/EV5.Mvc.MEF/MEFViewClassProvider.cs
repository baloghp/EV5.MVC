using EV5.Mvc.Services;
using EV5.Mvc.ViewEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.MEF
{
    public class MEFViewClassProvider : IViewClassProvider
    {
        public  IEmbeddedView GetEmbeddedViewClass(string viewName)
        {
            var views = EV5MefCompositionHost.CompositionHost.GetExports(typeof(IEmbeddedView),viewName);
            if (views != null && views.Count() > 0)
            {
                var view = views.First();
                return view as IEmbeddedView;
            }
            return null;
        }

        
    }
}
