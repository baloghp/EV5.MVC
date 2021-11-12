using EV5.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Samples.Embedded.Controllers
{
    public class SamplesController : Controller
    {

        public ActionResult RetrieveHtmlResult()
        {
            return new EmbeddedHtmlStringResult("EV5.Samples.Embedded.Assets.LandingPage.index.html", this.GetType().Assembly);
        }

        public ActionResult RetrieveSimpleRazor()
        {

            return View("EV5.Samples-Views.Sample.Index.cshtml");

        }
    }

}
