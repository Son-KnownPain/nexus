using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class RetailStoresController : Controller
    {
        // GET: RetailStores
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DetailsRetailsStore()
        {
            return View();
        }
    }
}