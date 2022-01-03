using EV5.Mvc;
using EV5.Mvc.MEF;
using EV5.Mvc.ViewEngine;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Samples.ViewEngine.Views.Extensions
{
    [MarkupName("EV5.VE-Assets.Sample.Extensions.LandingPageMaster.html")]
    [EmbeddedView("Extensions.LandingPageMaster")]
    public class LandingPageMasterView : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            var node = this.HtmlDocument
                             .Document
                             .SelectSingleNode("//h1");
            if (node != null) node.InnerHtml = "Changed in the Master view";
             node = this.HtmlDocument
                             .Document
                             .SelectSingleNode("//*[@id=\"mainNav\"]/div/a");
            
            if (node != null) node.InnerHtml = "Changed in the Master view";
           
        }
    }
}
