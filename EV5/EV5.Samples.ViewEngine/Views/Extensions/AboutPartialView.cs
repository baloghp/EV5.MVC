using EV5.Mvc;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EV5.Samples.ViewEngine.Views.Extensions
{
    [MarkupName("EV5.VE-Assets.Sample.Extensions.About.html")]
    [EmbeddedView("Extensions.AboutPartial")]
    public class LAboutpartialView : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            var node = this.HtmlDocument
                             .Document
                             .SelectSingleNode("//h1");
            if (node != null) node.InnerHtml = "Changed in the About Partial view";

            //*[@id="page-top"]/footer/div/div
            node = this.HtmlDocument
                            .Document
                            .SelectSingleNode(" //h2");
            if (node != null) node.InnerHtml = "Changed in the About Partial view";
        }
    }
}