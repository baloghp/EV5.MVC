﻿using System;
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
            return View("eve-Assets.Views.Sample.SimpleHtml.LandingPage.html");
        }

        public ActionResult ShowCodeHtml()
        {
            return View("eve-Just.Code.Simple.Html.View");
        }

        public ActionResult ShowSimpleMasterHtml()
        {
            return View("eve-Assets.Views.Sample.SimpleMaster.LandingPage.html");
        }

        public ActionResult ShowSimpleMasterAndPartial()
        {
            return View("eve-Assets.Views.Sample.MasterAndPartials.LandingPage.html");
        }

        public ActionResult Sections()
        {
            return View("eve-Assets.Views.Sample.Sections.LandingPage.html");
        }
        public ActionResult Bundles()
        {
            return View("eve-Assets.Views.Sample.Bundles.LandingPage.html");
        }
        public ActionResult Typed()
        {
            return View("eve-Assets.Views.Sample.Typed.LandingPage.html", Models.LandingPageModel.GetSample());
        }
        public ActionResult DataBinding()
        {
            this.ViewBag.Title = "Title comes from viewbag!!!!";
            return View("eve-Assets.Views.Sample.DataBinding.LandingPage.html", Models.LandingPageModel.GetSample());
        }

        public ActionResult BundlesInJapan()
        {

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ja-JP");
            return View("eve-Assets.Views.Sample.Bundles.LandingPage.html");
        }

        public ActionResult ShortViewNames()
        {
            this.ViewBag.Title = "Title comes from viewbag!!!!";
            return View("eve-ShortViewNames.LandingPage.html", Models.LandingPageModel.GetSample());
        }
    }
}
