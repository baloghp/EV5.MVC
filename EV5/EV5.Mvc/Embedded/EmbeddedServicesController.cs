using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace EV5.Mvc.Embedded
{
    
    public class EmbeddedServicesController : Controller
    {
        
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
        private readonly IHostEnvironment _env;
        
        public EmbeddedServicesController( IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IHostEnvironment env)
        {
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _env = env;
            
        }
       

        [HttpGet]
        public JsonResult GetContentRootDirectoryContents()
        {
            var contents = _env.ContentRootFileProvider.GetDirectoryContents(string.Empty);
            return new JsonResult(contents);
            
            
        }
        //public ActionResult<List<IFileInfo>> GetServicedFileProviderContents()
        //{
        //   // var contents = this._servicedProvider.GetDirectoryContents(string.Empty);
        //   // return contents.ToList();

        //}

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
