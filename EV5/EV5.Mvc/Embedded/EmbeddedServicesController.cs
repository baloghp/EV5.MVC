using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EV5.Mvc.Embedded
{
    
    public class EmbeddedServicesController : Controller
    {
        private readonly IFileProvider fileprovider;
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
        private readonly IHostingEnvironment _env;
        public EmbeddedServicesController(IFileProvider fileprovider, 
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            //cannot get IWebHostEnvironment being recognized here, although its partner is available,
            //this function will be deprecated once this type disappears
            IHostingEnvironment env)
        {
            this.fileprovider = fileprovider;
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _env = env;
        }
       

        [HttpGet]
        public ActionResult<List<IFileInfo>> GetContentRootDirectoryContents()
        {
            var contents = _env.ContentRootFileProvider.GetDirectoryContents(string.Empty);
            return contents.ToList();
            
        }
        public ActionResult<List<IFileInfo>> GetWebRootDirectoryContents()
        {
            var contents = this.fileprovider.GetDirectoryContents(string.Empty);
            return contents.ToList();

        }

        public IActionResult GetActions()
        {
            return Ok(actionDescriptorCollectionProvider
                .ActionDescriptors
                .Items
                .OfType<ControllerActionDescriptor>()
                .Select(a => new
                {
                    a.DisplayName,
                    a.ControllerName,
                    a.ActionName,
                    AttributeRouteTemplate = a.AttributeRouteInfo?.Template,
                    //HttpMethods = string.Join(", ", a.ActionConstraints?.OfType<HttpMethodActionConstraint>().SingleOrDefault()?.HttpMethods ?? new string[] { "any" }),
                    Parameters = a.Parameters?.Select(p => new
                    {
                        Type = p.ParameterType.Name,
                        p.Name
                    }),
                    ControllerClassName = a.ControllerTypeInfo.FullName,
                    ActionMethodName = a.MethodInfo.Name,
                    Filters = a.FilterDescriptors?.Select(f => new
                    {
                        ClassName = f.Filter.GetType().FullName,
                        f.Scope //10 = Global, 20 = Controller, 30 = Action
                    }),
                    Constraints = a.ActionConstraints?.Select(c => new
                    {
                        Type = c.GetType().Name
                    }),
                    RouteValues = a.RouteValues.Select(r => new
                    {
                        r.Key,
                        r.Value
                    }),
                }));
        }
    }
}
