using EV5.Mvc;
using EV5.Mvc.ViewEngine;
using EV5.Mvc.MEF;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.AdminLTE.Views
{
    [MasterView("ev5.alte-AdminLTE.master")]
    [MarkupName("EV5.ALTE-AdminLTE.Views.index.html")]
    [EmbeddedView("AdminLTE.index")]
    public class IndexView : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            ;
        }
    }
}
