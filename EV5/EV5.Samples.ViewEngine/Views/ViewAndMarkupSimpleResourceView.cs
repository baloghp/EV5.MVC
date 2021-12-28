using EV5.Mvc;
using EV5.Mvc.MEF;
using EV5.Mvc.ViewEngine;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EV5.Samples.ViewEngine.Views
{
    [EmbeddedView("View.and.Markup.Simple.Html.View")]
    [MarkupName("EV5.VE-Assets.Sample.SimpleHtml.index.html")]
    public class ViewAndMarkupSimpleResourceView : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            var node = this.HtmlDocument
                            .Document
                            .SelectSingleNode("//h1");
            node.InnerHtml = "This is new generated content";
          
        }
    }
}
