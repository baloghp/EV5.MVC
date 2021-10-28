using EV5.Mvc.ViewEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.Services
{
    /// <summary>
    /// Base Markup provider abstraction
    /// </summary>
    public  interface IMarkupProvider 
    {
        /// <summary>
        /// Gets the markup based on the viewname and the view class.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="view">The view class, can be null.</param>
        /// <returns></returns>
        public  string GetResource(string viewName, IEmbeddedView view);
    }
}
