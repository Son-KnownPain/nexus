using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using eProject.Models;
using eProject.Models.ViewModels.CallCharge;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class CallChargeController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/CallCharge
        public ActionResult ListOf(int? paymentPlanDetailID)
        {
            if (paymentPlanDetailID == null) return Redirect("/Admin");

            ViewBag.listCallCharge = context.CallCharges.Join(context.CallChargeTypes, cc => cc.CallChargeTypeID, cct => cct.CallChargeTypeID, (cc, cct) => new CallChargeViewModel
            {
                CallChargeID = cc.CallChargeID,
                PaymentPlanDetailID = cc.PaymentPlanDetailID,
                CallChargeTypeID = cct.CallChargeTypeID,
                CallChargeName = cct.TypeName,
                CostPerMinute = cc.CostPerMinute,
            }).Where(m => m.PaymentPlanDetailID == paymentPlanDetailID).ToList();

            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(p => p.PaymentPlanDetailID == paymentPlanDetailID);

            if (pld != null)
            {
                ViewBag.plID = pld.PaymentPlanID;
                ViewBag.pldID = pld.PaymentPlanDetailID;
                ViewBag.PaymentPlanDetailName = pld.Content;

                PaymentPlan pl = context.PaymentPlans.FirstOrDefault(p => p.PaymentPlanID == pld.PaymentPlanID);

                ViewBag.PaymentPlanName = pl.PlanName;
                ViewBag.ServiceName = context.Services.FirstOrDefault(s => s.ServiceID == pl.ServiceID).ServiceName;
            } else
            {
                return Redirect("/Admin");
            }

            return View();
        }

        // GET: Admin/CallCharge/Add
        public ActionResult Add(int? paymentPlanDetailID)
        {
            if (paymentPlanDetailID == null) return Redirect("/Admin");

            if (context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == paymentPlanDetailID) == null)
            {
                return Redirect("/Admin");
            }

            int pldIDInt = (int)paymentPlanDetailID;

            CallChargeViewModel model = new CallChargeViewModel();

            model.PaymentPlanDetailID = pldIDInt;

            return View(model);
        }

        // POST: Admin/CallCharge/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Store(CallChargeViewModel data)
        {
            if (!ModelState.IsValid) return View("Add");

            CallCharge callCharge = new CallCharge();

            callCharge.CostPerMinute = data.CostPerMinute;
            callCharge.CallChargeTypeID = data.CallChargeTypeID;
            callCharge.PaymentPlanDetailID = data.PaymentPlanDetailID;

            context.CallCharges.Add(callCharge);

            context.SaveChanges();

            TempData["Success"] = "Successfully add new call charge";

            return Redirect("/Admin/CallCharge/ListOf?paymentPlanDetailID=" + data.PaymentPlanDetailID);
        }

        // GET: Admin/CallCharge/Edit
        public ActionResult Edit(int? callChargeID)
        {
            if (callChargeID == null) return Redirect("/Admin");

            CallCharge callCharge = context.CallCharges.FirstOrDefault(c => c.CallChargeID == callChargeID);

            if (callCharge == null) return Redirect("/Admin");

            CallChargeViewModel callChargeViewModel = new CallChargeViewModel();
            callChargeViewModel.CallChargeID = callCharge.CallChargeID;
            callChargeViewModel.PaymentPlanDetailID = callCharge.PaymentPlanDetailID;
            callChargeViewModel.CallChargeTypeID = callCharge.CallChargeTypeID;
            callChargeViewModel.CostPerMinute = callCharge.CostPerMinute;

            return View(callChargeViewModel);
        }

        // PUT: Admin/CallCharge/Update
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CallChargeViewModel data)
        {
            if (!ModelState.IsValid) return View("Add");

            CallCharge callCharge = context.CallCharges.FirstOrDefault(m => m.CallChargeID == data.CallChargeID);

            if (callCharge == null) return Redirect("/Admin");

            callCharge.CostPerMinute = data.CostPerMinute;
            callCharge.CallChargeTypeID = data.CallChargeTypeID;
            callCharge.PaymentPlanDetailID = data.PaymentPlanDetailID;

            context.SaveChanges();

            TempData["Success"] = "Successfully update call charge";

            return Redirect("/Admin/CallCharge/ListOf?paymentPlanDetailID=" + callCharge.PaymentPlanDetailID);
        }

        // GET: Admin/CallCharge/Delete
        public ActionResult Delete(int? callChargeID)
        {
            if (callChargeID == null) return Redirect("/Admin");

            CallCharge callCharge = context.CallCharges.FirstOrDefault(c => c.CallChargeID == callChargeID);

            if (callCharge == null) return Redirect("/Admin");

            context.CallCharges.Remove(callCharge);

            context.SaveChanges();

            TempData["Success"] = "Successfully delete call charge";

            return Redirect("/Admin/CallCharge/ListOf?paymentPlanDetailID=" + callCharge.PaymentPlanDetailID);
        }
    }
}