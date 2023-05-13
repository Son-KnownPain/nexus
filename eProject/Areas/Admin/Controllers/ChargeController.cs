using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Areas.Admin.Controllers
{
    public class ChargeController : Controller
    {
        // GET: Admin/Charge
        public ActionResult Add()
        {
            return View();
        }
    }
}