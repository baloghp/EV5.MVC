using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EV5.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EV5.Samples.ViewEngine.Controllers
{
    public class SampleController : Controller
    {
        public ActionResult ShowResourceHtml()
        {
            return View("eve-EV5.VE-Assets.Sample.SimpleHtml.index.html");
        }
        public ActionResult ShowViewAndMarkupSimpleResourceHtml()
        {
            return View("eve-View.and.Markup.Simple.Html.View");
        }
        public ActionResult ShowCodeHtml()
        {
            return View("eve-Just.Code.Simple.Html.View");
        }

        public ActionResult ShowSimpleMasterHtml()
        {
            return View("eve-SimpleMaster.LandingPage");
        }

        public ActionResult ShowSimpleMasterAndPartial()
        {
            
            return View("eve-MasterAndPartials.LandingPage");
        }

        public ActionResult Sections()
        {
            return View("eve-Sections.LandingPage");
        }
       
        public ActionResult Typed()
        {
            return View("eve-Typed.LandingPage", Models.LandingPageModel.GetSample());
        }
        public ActionResult Extensions()
        {
            this.ViewBag.Content = "This content comes from viewbag!!!!";
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ja-JP");
            return View("eve-Extensions.LandingPage", Models.LandingPageModel.GetSample());
        }

        
    }
}
