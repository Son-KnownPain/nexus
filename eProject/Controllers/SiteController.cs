using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class SiteController : Controller
    {
        // GET: Site
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HowToOrderAService()
        {
            return View();
        }
        
        public ActionResult Faq()
        {
            return View();
        }
    }
}