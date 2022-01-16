using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
using EV5.Mvc.MEF;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.Plugin;
using EV5.Mvc.Embedded;

namespace EV5.Mvc.Utils
{

    public class EmbeddedServicesController : Controller
    {

        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
        private readonly IWebHostEnvironment _env;
        public EmbeddedServicesController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IWebHostEnvironment env)
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
            return Ok(_env.WebRootFileProvider.GetDirectoryContents(string.Empty).Select(a => new
            {
                a.Name,
                a.PhysicalPath,
                a.Length,
                a.Exists,
                a.IsDirectory,
                a.LastModified
            }));
        }

        public ActionResult GetAllEV5Plugins()
        {
            var classesAndImplementation =
             from a in AppDomain.CurrentDomain.GetAssemblies()
             from t in a.GetTypes()
             where typeof(IEmbeddedPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract
             let impl = Activator.CreateInstance(t)
             select new
             {
                 Type = t,
                 Implementation = (IEmbeddedPlugin)impl,
             };

            return Ok(classesAndImplementation.Select(a => new
            {

                pluginType = a.Type.FullName,
                pluginAssembly = a.Type.Assembly.FullName,
                webpartsAssembly = a.Implementation.WebPartsAssembly.FullName,
                fileProvider = a.Implementation.FileProvider.GetType().FullName,
                embeddedfileProviderPrefix = (a.Implementation.FileProvider as EV5EmbeddedFileProvider)?.Prefix,
                embeddedfileProviderAssembly = (a.Implementation.FileProvider as EV5EmbeddedFileProvider)?.Assembly.FullName,
                insertOwnEmbeddedViewEngine = a.Implementation.InsertOwnEmbeddedViewEngine,
                ownEmbeddedViewEnginePrefix = a.Implementation.OwnEmbeddedViewEnginePrefix
            }));
        }

        public IActionResult GetEv5ViewNames()
        {
            var classesAndAttribute =
             from a in AppDomain.CurrentDomain.GetAssemblies()
             from t in a.GetTypes()
             let attributes = t.GetCustomAttributes(typeof(EmbeddedViewAttribute), true)
             where attributes != null && attributes.Length > 0
             let viewNameAttribute = t.GetCustomAttributes(typeof(EmbeddedViewAttribute), true).First()
             where viewNameAttribute != null
             select new
             {
                 Type = t,
                 ViewNameAttribute = (EmbeddedViewAttribute)viewNameAttribute,
             };
            
            return Ok(classesAndAttribute.Select(a => new
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
