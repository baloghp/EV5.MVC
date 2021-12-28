using EV5.Mvc;
using EV5.Mvc.MEF;
using EV5.Mvc.ViewEngine;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Samples.ViewEngine.Views.SimpleMaster
{
    [MasterView("eve-SimpleMaster.LandingPageMaster")]
    [MarkupName("EV5.VE-Assets.Sample.SimpleMaster.LandingPage.html")]
    [EmbeddedView("SimpleMaster.LandingPage")]
    public class LandingPageView : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            var node = this.HtmlDocument
                             .Document
                             .SelectSingleNode("//h1");
            if (node != null)  node.InnerHtml = "Changed in the Page view";

            //*[@id="page-top"]/footer/div/div
            node = this.HtmlDocument
                            .Document
                            .SelectSingleNode(" //footer/div/div");
            if (node != null) node.InnerHtml = "Changed in the Page view";
        }
    }
}
