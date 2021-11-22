using EV5.Mvc.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc.ViewEngine.Providers
{
    /// <summary>
    /// Markup provider that returns the views if the view name refers to a valid embedded html, or snippet of html.
    /// </summary>
    public class EmbeddedMarkupProvider : IMarkupProvider
    {
        IFileProvider _fileprovider;
        public EmbeddedMarkupProvider(IWebHostEnvironment env )
        {
            _fileprovider = env.ContentRootFileProvider;
        }

        public  string GetResource(string viewName, IEmbeddedView view)
        {

            string markup = String.Empty;
            //first let's try to get it from the defulat file system
            var fileinfo = _fileprovider.GetFileInfo(viewName);
            if (fileinfo.Exists)
            {
                using (var sr = new StreamReader(fileinfo.CreateReadStream()))
                {
                    markup = sr.ReadToEnd();
                }
                return markup;
            }

            // first let's try if the code and the resource are in the same assembly, 
            //otherwise we have to figure out which assembly the view belongs to
            if (view != null)
            {
                markup = AssetManager.LoadResourceString(viewName, view.GetType().Assembly);
            }
            //if we could not find it there or there is no class specified let's try by searching everywhere
            if (string.IsNullOrWhiteSpace(markup))
            {
                markup = AssetManager.LoadResourceString(viewName);
            };
            return markup;
        }
    }
}
