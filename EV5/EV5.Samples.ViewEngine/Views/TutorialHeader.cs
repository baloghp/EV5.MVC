using EV5.Mvc;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EV5.Samples.ViewEngine.Views
{
    [MarkupName("EV5.VE-Assets.Tutorial.Shared.Head.html")]
    [EmbeddedView("EV5.VE-Assets.Tutorial.Shared.Head.html")]
    public class TutorialHeader : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            var node1 = this.HtmlDocument
                             .Document
                             .SelectSingleNode("//title");
            
            var node2 = this.Parent.HtmlDocument
                            .Document
                            .SelectSingleNode(" //h2");
            if ((node1! != null) && (node2 != null)) node1.InnerHtml = node2.InnerHtml;
        }
    }
}
