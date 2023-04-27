using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;

namespace eProject.Areas.Admin.Controllers
{
    [EmployeeAuthorization]
    public class SiteController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
    }
}