using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EV5.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EV5.Samples.ViewEngine.Controllers
{
    public class TutorialController : Controller
    {

        public ActionResult RegisterViewEngine()
        {
            return View("eve-EV5.VE-Assets.Tutorial.RegisterViewEngine.html");
        }
        public ActionResult SimpleHtml()
        {
            return View("eve-EV5.VE-Assets.Tutorial.SimpleHtml.html");
        }
        public ActionResult SimpleMaster()
        {
            return View("eve-EV5.VE-Assets.Tutorial.SimpleMaster.SimpleMaster.html");
        }
        public ActionResult SimpleMasterAndPartial()
        {
            return View("eve-EV5.VE-Assets.Tutorial.MasterAndPartials.MasterAndPartials.html");
        }

        public ActionResult Sections()
        {
            return View("eve-EV5.VE-Assets.Tutorial.Sections.html");
        }

        public ActionResult Bundles()
        {
            return View("eve-EV5.VE-Assets.Tutorial.Bundles.html");
        }

        public ActionResult TypedViews()
        {
            return View("eve-EV5.VE-Assets.Tutorial.Typed.html");
        }
        public ActionResult DataBinding()
        {
            return View("eve-EV5.VE-Assets.Tutorial.DataBinding.html");
        }

        public ActionResult Localization()
        {
            return View("eve-EV5.VE-Assets.Tutorial.Localization.html");
        }

        public ActionResult EveAttributes()
        {
            return View("eve-EV5.VE-Assets.Tutorial.EveAttributes.html");
        }

        public ActionResult ExtendingEve()
        {
            return View("eve-EV5.VE-Assets.Tutorial.ExtendingEve.html");
        }

        public ActionResult ShortViewNames()
        { 
            return View("eve-EV5.VE-Assets.Tutorial.ShortViewNames.html");
        }
    }
}
