using EV5.Mvc;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;
using EV5.Samples.ViewEngine.Models;
using EV5.Mvc.Processor.Extensions;

namespace EV5.Samples.ViewEngine.Views.Typed
{
    [MasterView("eve-Typed.LandingPageMaster")]
    [MarkupName("EV5.VE-Assets.Sample.Typed.LandingPage.html")]
    [EmbeddedView("Typed.LandingPage")]
    public class LandingPageView : EmbeddedView<LandingPageModel>
    {
        protected override void BeforeProcessView(ViewContext viewContext)
        {
            base.BeforeProcessView(viewContext);
            this.HtmlDocument.Document
                .ProcessEvals(this.Model);
        }
        public override void ProcessView(ViewContext viewContext)
        {

            var node = this.HtmlDocument
                             .Document
                             .SelectSingleNode("//h1");
            if (node != null) node.InnerHtml = Model.Header.Header1;

            
        }
    }
}
