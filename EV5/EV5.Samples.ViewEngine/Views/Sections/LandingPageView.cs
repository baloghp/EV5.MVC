using EV5.Mvc;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EV5.Samples.ViewEngine.Views.Sections
{
    [MasterView("eve-Sections.LandingPageMaster")]
    [MarkupName("EV5.VE-Assets.Sample.Sections.LandingPage.html")]
    [EmbeddedView("Sections.LandingPage")]
    public class LandingPageView : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            var node = this.HtmlDocument
                             .Document
                             .SelectSingleNode("//h1");
            if (node != null) node.InnerHtml = "Changed in the Page view";

            node = this.HtmlDocument
                            .Document
                            .SelectSingleNode(" //footer/div/div");
            if (node != null) node.InnerHtml = "Changed in the Page view";

            node = this.HtmlDocument
                           .Document
                           .SelectSingleNode("//*[@id=\"ctatext\"]");
            if (node != null) node.InnerHtml = "Changed in the Page view";
        }
    }
}
