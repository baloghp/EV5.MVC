using EV5.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Samples.Embedded.Controllers
{
    public class TutorialController : Controller
    {
        public ActionResult RetrieveHtml()
        {
            return new EmbeddedHtmlStringResult("EV5.Samples.Embedded.Assets.Tutorial.EFS.Setup.Setup.html",
                this.GetType().Assembly);
        }

      
    }
}
