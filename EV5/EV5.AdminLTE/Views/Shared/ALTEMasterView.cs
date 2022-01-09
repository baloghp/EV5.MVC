using EV5.Mvc;
using EV5.Mvc.MEF;
using EV5.Mvc.ViewEngine;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.AdminLTE.Views.Shared
{
    [MarkupName("EV5.ALTE-AdminLTE.Views.Shared.ALTEMaster.html")]
    [EmbeddedView("AdminLTE.master")]
    public class ALTEMasterView : EmbeddedView
    {
        public override void ProcessView(ViewContext viewContext)
        {
            ;
        }
    }
}
