﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
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
