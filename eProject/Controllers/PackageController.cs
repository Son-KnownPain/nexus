using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class PackageController : Controller
    {
        // GET: Package/Detail
        public ActionResult Detail(int? packageId)
        {
            return View();
        }
    }
}