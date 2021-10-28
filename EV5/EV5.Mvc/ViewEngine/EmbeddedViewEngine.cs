using EV5.Mvc.Extensions;
using EV5.Mvc.ViewEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
//using EVE.Mvc.ViewEngine.Providers;
using System;
using System.Reflection;


namespace EV5.Mvc
{

    /// <summary>
    /// View engine implementation. This class is responsible to find markup and code parts of the views, and provide appropriate fall-backs should one be missing.
    /// </summary>
    public class EmbeddedViewEngine : IViewEngine
    {

        /// <summary>
        /// Gets or sets the view name prefix, of the view engine
        /// </summary>
        /// <value>
        /// The view name prefix.
        /// </value>
        public string ViewNamePrefix { get; set; }

       

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedViewEngine"/> class.
        /// </summary>
        /// <param name="viewNamePrefix">The view name prefix.</param>
        public EmbeddedViewEngine(string viewNamePrefix = ""
            )
        {
           
            ViewNamePrefix = viewNamePrefix;

        }

        /// <summary>
        /// Finds the specified partial view by using the specified controller context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="useCache">true to specify that the view engine returns the cached view, if a cached view exists; otherwise, false.</param>
        /// <returns>
        /// The partial view.
        /// </returns>
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            IEmbeddedView view = GetView(partialViewName);
            if (view != null)
            {
                view.ViewName = partialViewName;
                return ViewEngineResult.Found(partialViewName, view);
            }
            return ViewEngineResult.NotFound(partialViewName,new string[] { partialViewName });
        }

        /// <summary>
        /// Finds the specified view by using the specified controller context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewName">The name of the view.</param>
        /// <param name="masterName">The name of the master.</param>
        /// <param name="useCache">true to specify that the view engine returns the cached view, if a cached view exists; otherwise, false.</param>
        /// <returns>
        /// The page view.
        /// </returns>
        //public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        //{
        //    IEmbeddedView view = GetView(viewName);
        //    if (view != null)
        //    {
        //        view.ViewName = viewName;
        //        //set master name only if there is one specified,
        //        //otherwise let the view try to figure out by its attributes
        //        if (!string.IsNullOrWhiteSpace(masterName))
        //        {
        //            view.MasterName = masterName;
        //        }

        //        return new ViewEngineResult(view, this);
        //    }
        //    return new ViewEngineResult(new string[] { masterName, viewName });
        //}
        /// <summary>
        /// So! A typical view is made up of an EmbeddedView class and a pice of markup string.
        /// First we look for the class and then pass the markup to it. 
        /// We're going to assume that if a class exists the corresponding markup will be in proximity to it (same assembly).
        /// So we're going to pass both the view name and the view class to the markup provider. 
        /// The default markup provider will look for the markup as an embedded string in the view class's assembly.
        /// If we do not find a class we still look for a markup using the same provider 
        /// (so the providers must be prepared to handle the scenario  when now view class is passed)
        /// and pass it to a "empty" embedded view.
        /// If we find a class but no markup we just pass the class on.
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private IEmbeddedView GetView(string viewName)
        {
            string realViewName = UnprefixViewName(viewName);
            if (string.IsNullOrWhiteSpace(realViewName)) return null;
            IEmbeddedView view = FindEmbeddedViewClass(viewName);

            string markup = FindMarkup(realViewName, view);
            if (view != null)
            {
                view.ViewEngine = this;
                if (!(string.IsNullOrEmpty(markup)))
                {
                    view.RawMarkup = markup;
                }
                return view;
            }
            if (!(string.IsNullOrEmpty(markup)))
            {
                var sview = new SimpleResourceView();
                sview.ViewEngine = this;
                sview.RawMarkup = markup;
                return sview;
            }

            return null;
        }

        private string UnprefixViewName(string viewName)
        {
            if (!String.IsNullOrWhiteSpace(this.ViewNamePrefix))
            {
                if (viewName.StartsWith(this.ViewNamePrefix))
                {
                    return viewName.Remove(0, this.ViewNamePrefix.Length);
                }
                else
                    return string.Empty;
            }
            return viewName;
        }

        private string FindMarkup(string viewName, IEmbeddedView view)
        {
            var markupProvider = ServicesExtensions.MarkupProvider;

            if (view != null)
            {
                //let's see if there is a markup name specified for the view class
                var viewAttribute = view.GetType().GetCustomAttribute<MarkupNameAttribute>();
                if (viewAttribute != null && !String.IsNullOrWhiteSpace(viewAttribute.MarkupName))
                {
                    string result = markupProvider.GetResource(viewAttribute.MarkupName, view);
                    if (!String.IsNullOrWhiteSpace(result)) return result;
                }
            }


            return markupProvider.GetResource(viewName, view);
        }

        private IEmbeddedView FindEmbeddedViewClass(string viewName)
        {
            return ViewClassProvider.GetEmbeddedViewClass(viewName);
        }

        /// <summary>
        /// Releases the specified view by using the specified controller context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="view">The view.</param>
        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var embeddedView = view as IEmbeddedView;
            if (view != null)
            {
                embeddedView.CleanUp();
            }
            //not too sure if we need this!
            //GC.Collect();
        }
        //
        // Summary:
        //     Finds the view with the given viewName using view locations and information from
        //     the context.
        //
        // Parameters:
        //   context:
        //     The Microsoft.AspNetCore.Mvc.ActionContext.
        //
        //   viewName:
        //     The name or path of the view that is rendered to the response.
        //
        //   isMainPage:
        //     Determines if the page being found is the main page for an action.
        //
        // Returns:
        //     The Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult of locating the view.
        //
        // Remarks:
        //     Use Microsoft.AspNetCore.Mvc.ViewEngines.IViewEngine.GetView(System.String,System.String,System.Boolean)
        //     when the absolute or relative path of the view is known.
        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            IEmbeddedView view = GetView(viewName);
            if (view != null)
            {
                view.ViewName = viewName;
                //set master name only if there is one specified,
                //otherwise let the view try to figure out by its attributes
                //if (!string.IsNullOrWhiteSpace(masterName))
                //{
                //    view.MasterName = masterName;
                //}

                return ViewEngineResult.Found(viewName, view);
            }
            return ViewEngineResult.NotFound(viewName, new string[] { viewName });

        }
        //
        // Summary:
        //     Gets the view with the given viewPath, relative to executingFilePath unless viewPath
        //     is already absolute.
        //
        // Parameters:
        //   executingFilePath:
        //     The absolute path to the currently-executing view, if any.
        //
        //   viewPath:
        //     The path to the view.
        //
        //   isMainPage:
        //     Determines if the page being found is the main page for an action.
        //
        // Returns:
        //     The Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult of locating the view.
        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            throw new NotImplementedException();
        }
    }


}
