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
using EV5.Mvc.MEF;
using EV5.Mvc.ViewEngine;
using System.Composition.Hosting;
using EV5.Mvc.Extensions;

namespace EV5.Mvc.Embedded
{

    public class EmbeddedServicesController : Controller
    {

        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
        private readonly IWebHostEnvironment _env;
        private CompositionHost compositionHost;
        public EmbeddedServicesController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IWebHostEnvironment env, CompositionHost compositionHost)
        {
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _env = env;
        }


        public IActionResult GetContentRootDirectoryContents()
        {
            return Ok(_env.ContentRootFileProvider.GetDirectoryContents(string.Empty).Select(a => new
            {
                a.Name,
                a.PhysicalPath,
                a.Length,
                a.Exists,
                a.IsDirectory,
                a.LastModified
            }));
        }
        public IActionResult GetWebRootDirectoryContents()
        {
            return Ok(this._env.WebRootFileProvider.GetDirectoryContents(string.Empty).Select(a => new
            {
                a.Name,
                a.PhysicalPath,
                a.Length,
                a.Exists,
                a.IsDirectory,
                a.LastModified
            }));
        }

        public IActionResult GetEv5ViewNames()
        {
            var classesAndAttributes=
             from a in AppDomain.CurrentDomain.GetAssemblies()
             from t in a.GetTypes()
             let attributes = t.GetCustomAttributes(typeof(EmbeddedViewAttribute), true)
             where attributes != null && attributes.Length > 0
             let viewNameAttribute = t.GetCustomAttributes(typeof(EmbeddedViewAttribute), true).First()
             where viewNameAttribute != null
             select new { Type = t, 
                 ViewNameAttribute = (EmbeddedViewAttribute)viewNameAttribute, 
             };

            return Ok(classesAndAttributes.Select(a => new
            {
                
                viewName = a.ViewNameAttribute.ContractName,
                masterName = GetMasterName(a.Type),
                markupName = GetMarkupName(a.Type),
                modelType = GetModelType(a.Type),
                viewType = a.Type.FullName,
                baseType = a.Type.BaseType.FullName
            })); ;
        }

        private object GetModelType(Type type)
        {
            var gens = type.BaseType.GetGenericArguments();
            if (gens == null || gens.Length == 0) return default;
            return gens.First().FullName;
        }

        private object GetMarkupName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(MarkupNameAttribute), true);
            if (attr == null || attr.Length == 0) return default;
            return ((MarkupNameAttribute)attr.First()).MarkupName;
        }

        private string GetMasterName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(MasterViewAttribute), true);
            if (attr == null || attr.Length == 0) return default;
            return ((MasterViewAttribute)attr.First()).MasterViewName;
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

        public ActionResult CallView(string viewname)
        {
            return View(viewname);
        }
    }
}
