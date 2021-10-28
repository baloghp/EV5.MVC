﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EV5.Mvc.ViewEngine
{
    /// <summary>
    /// EMbeddedView implementation for the case when no view code is provided.
    /// </summary>
    public class SimpleResourceView: EmbeddedView
    {
      
        public override void ProcessView(ViewContext viewContext)
        {
            //do nothing
        }
    }
}
