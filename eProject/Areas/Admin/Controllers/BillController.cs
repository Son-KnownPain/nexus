using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Areas.Admin.Controllers
{
    public class BillController : Controller
    {
        // GET: Admin/Bill
        public ActionResult Index()
        {
            return View();
        }
    }
}