using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Models;

namespace eProject.Areas.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Admin/Employee/SignIn
        public ActionResult SignIn()
        {
            return View();
        }

        // POST: Admin/Employee/Check
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Check(SimpleProxyEmployee data)
        {
            if (!ModelState.IsValid) return View("SignIn");
            return Redirect("/Admin");
        }
    }
}