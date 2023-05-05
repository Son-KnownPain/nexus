using eProject.Models;
using eProject.Models.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class OrderController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Order/Prepare
        public ActionResult Prepare(int? serviceID, int? paymentPlanDetailID)
        {
            if (serviceID == null || paymentPlanDetailID == null) return Redirect("/");

            Service service = context.Services.FirstOrDefault(s => s.ServiceID == serviceID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(a => a.PaymentPlanDetailID == paymentPlanDetailID);

            if (service == null || pld == null) return Redirect("/");

            ViewBag.service = service;
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(a => a.PaymentPlanID == pld.PaymentPlanID);

            return View();
        }

        // POST: Admin/Order/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Store(OrderRegisterViewModel data)
        {
            if (!ModelState.IsValid) return View("Prepare");

            // Handle create new customer

            // Handle create new order
            Order order = new Order();
            order.ServiceID = data.ServiceID;
            order.PaymentPlanDetailID = data.PaymentPlanDetailID;

            // Login

            // Redirect to customer's order
            return Redirect("/");
        }
    }
}