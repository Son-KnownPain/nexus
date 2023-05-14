using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Models.ViewModels.PaymentPlanDetails;
using eProject.Models;
using eProject.Filters;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class PaymentPlanDetailController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/PaymentPlanDetail/Add
        public ActionResult Add(int? paymentPlanID)
        {
            if (paymentPlanID == null) return Redirect("/Admin/PaymentPlan");
            int paymentPlanIDInt = (int) paymentPlanID;
            return View(new PaymentPlanDetailViewModel
            {
                PaymentPlanID = paymentPlanIDInt
            });
        }

        // POST: Admin/PaymentPlanDetail/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Store(PaymentPlanDetailViewModel data)
        {
            if (!ModelState.IsValid) return View("Add");
            PaymentPlanDetail paymentPlanDetail = new PaymentPlanDetail();
            paymentPlanDetail.PaymentPlanID = data.PaymentPlanID;
            paymentPlanDetail.Content = data.Content;
            paymentPlanDetail.RentalCost = data.RentalCost;
            paymentPlanDetail.VadilityDays = data.VadilityDays;

            context.PaymentPlanDetails.Add(paymentPlanDetail);

            context.SaveChanges();

            TempData["Success"] = "Successfully add new payment plan detail";
            return Redirect("/Admin/PaymentPlan/Detail?paymentPlanID=" + data.PaymentPlanID);
        }

        // GET: Admin/PaymentPlanDetail/Edit
        public ActionResult Edit(int? paymentPlanDetailID)
        {
            if (paymentPlanDetailID == null) return Redirect("/Admin/PaymentPlan");
            PaymentPlanDetail model = context.PaymentPlanDetails
                .FirstOrDefault(m => m.PaymentPlanDetailID == paymentPlanDetailID);
            if (model == null) return Redirect("/Admin/PaymentPlan");
            return View(model);
        }

        // PUT: Admin/PaymentPlanDetail/Update
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update(PaymentPlanDetail data)
        {
            if (!ModelState.IsValid) return View("Add");

            PaymentPlanDetail paymentPlanDetail = context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == data.PaymentPlanDetailID);
            
            if (paymentPlanDetail == null) return Redirect("/Admin/PaymentPlan");

            paymentPlanDetail.Content = data.Content;
            paymentPlanDetail.RentalCost = data.RentalCost;
            paymentPlanDetail.VadilityDays = data.VadilityDays;

            context.SaveChanges();

            TempData["Success"] = "Successfully update payment plan detail";

            return Redirect("/Admin/PaymentPlan/Detail?paymentPlanID=" + data.PaymentPlanID);
        }

        public ActionResult Delete(int? paymentPlanDetailID)
        {
            if (paymentPlanDetailID == null)
            {
                return RedirectToAction("Index");
            }

            PaymentPlanDetail paymentPlanDetail = context.PaymentPlanDetails.FirstOrDefault(p => p.PaymentPlanDetailID == paymentPlanDetailID);
            if (paymentPlanDetail == null)
            {
                return RedirectToAction("Index");
            }




            //context.Bills.RemoveRange(context.Bills.Where(b => listCmt == paymentPlanDetailID));
            context.Accounts.RemoveRange(context.Accounts.Where(a => a.PaymentPlanDetailID == paymentPlanDetailID));
            context.CallCharges.RemoveRange(context.CallCharges.Where(c => c.PaymentPlanDetailID == paymentPlanDetailID));
            context.Orders.RemoveRange(context.Orders.Where(o => o.PaymentPlanDetailID == paymentPlanDetailID));

            context.PaymentPlanDetails.Remove(paymentPlanDetail);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete payment plan detail";
            return Redirect("Index");
        }
    }
}