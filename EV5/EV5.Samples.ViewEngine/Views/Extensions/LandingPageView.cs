using EV5.Mvc;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;
using EV5.Samples.ViewEngine.Models;
using EV5.Mvc.Processor.Extensions;
using EV5.Samples.ViewEngine.L10N;
using System.Threading;

namespace EV5.Samples.ViewEngine.Views.Extensions
{
    [MasterView("eve-Extensions.LandingPageMaster")]
    [MarkupName("EV5.VE-Assets.Sample.Extensions.LandingPage.html")]
    [EmbeddedView("Extensions.LandingPage")]
    public class LandingPageView : EmbeddedView<LandingPageModel>
    {
        protected override void BeforeProcessView(ViewContext viewContext)
        {
            base.BeforeProcessView(viewContext);
            this.HtmlDocument.Document
                .ProcessEvals(this.Model);

            this.HtmlDocument
                .ProcessViewBag(viewContext)
                .ProcessLocals(Resource.ResourceManager,Thread.CurrentThread.CurrentUICulture);
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
