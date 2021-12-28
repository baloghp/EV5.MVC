using EV5.Mvc;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EV5.Samples.ViewEngine.Views
{
    [EmbeddedView("Just.Code.Simple.Html.View")]
    public class SimpleHtmlView : EmbeddedView
    {

        public override void ProcessView(ViewContext viewContext)
        {
            this.HtmlDocument.Document.LoadHtml(@"<html>
                                                    <body>
                                                    Hello World!!!
                                                    </body>
                                                    </html>
                                                    ");
        }
    }
}
