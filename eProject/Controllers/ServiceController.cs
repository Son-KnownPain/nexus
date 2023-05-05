using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class ServiceController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Service
        public ActionResult Detail(int? serviceID)
        {
            if (serviceID == null) return Redirect("/");
            
            Service service = context.Services.FirstOrDefault(s => s.ServiceID == serviceID);

            if (service == null) return Redirect("/");

            ViewBag.pls = context.PaymentPlans.Where(p => p.ServiceID == service.ServiceID).ToList();

            return View(service);
        }
    }
}