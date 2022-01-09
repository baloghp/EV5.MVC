using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5.AdminLTE
{
    public class ALTEExampleController : Controller
    {
        public ActionResult CallView(string viewname)
        {
            return View(viewname);
        }
    }
}
