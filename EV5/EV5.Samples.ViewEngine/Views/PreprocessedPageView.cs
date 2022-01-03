using EV5.Mvc;
using EV5.Mvc.Processor.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace EV5.Samples.ViewEngine.Views
{
    public abstract class DataBindingView<T> : EmbeddedView<T>
    {
        protected override void BeforeProcessView(ViewContext viewContext)
        {
            base.BeforeProcessView(viewContext);
            this.HtmlDocument.Document.ProcessEvals(this.Model);
        }
    }
}
