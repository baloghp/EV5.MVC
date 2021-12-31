using EV5.Mvc;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;
using EV5.Mvc.Processor.Extensions;
using EV5.Samples.ViewEngine.Models;

namespace EV5.Samples.ViewEngine.Views.Typed
{
    [MarkupName("EV5.VE-Assets.Sample.Typed.Header.html")]
    [EmbeddedView("Typed.HeaderPartial")]
    public class HeaderView : EmbeddedView
    {

        protected override void BeforeProcessView(ViewContext viewContext)
        {
            base.BeforeProcessView(viewContext);
            HtmlDocument.Document.ProcessEvals((object)this.Model);
        }
        public override void ProcessView(ViewContext viewContext)
        {
            ;
        }
    }
}
