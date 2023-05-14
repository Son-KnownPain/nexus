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
                return Redirect("/Admin");
            }

            PaymentPlanDetail paymentPlanDetail = context.PaymentPlanDetails.FirstOrDefault(p => p.PaymentPlanDetailID == paymentPlanDetailID);

            if (paymentPlanDetail == null)
            {
                return Redirect("/Admin");
            }

            // Call charge -> PLD
            var callCharges = context.CallCharges.Where(x => x.PaymentPlanDetailID == paymentPlanDetail.PaymentPlanDetailID);

            // ------------------------------------------------------------

            // Order -> PLD
            var orders = context.Orders.Where(x => x.PaymentPlanDetailID == paymentPlanDetail.PaymentPlanDetailID);
            // Account -> Order
            var accountsOfOrder = context.Accounts.Where(x => orders.Select(o => o.OrderID).ToList().Contains(x.OrderID));
            // Bill -> Account
            var billsOfAccountsOfOrder = context.Bills.Where(x => accountsOfOrder.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // CustomerFeedback -> Account
            var cfOfAccountsOfOrder = context.CustomerFeedbacks.Where(x => accountsOfOrder.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // Charge -> Bill
            var chargesOfBillsOfAccountsOfOrder = context.Charges.Where(x => billsOfAccountsOfOrder.Select(a => a.BillID).ToList().Contains(x.BillID));

            // ------------------------------------------------------------

            // Account -> PLD
            var accounts = context.Accounts.Where(x => x.PaymentPlanDetailID == paymentPlanDetail.PaymentPlanDetailID);
            // Bill -> Account
            var billsOfAccounts = context.Bills.Where(x => accounts.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // CustomerFeedback -> Account
            var cfOfAccounts = context.CustomerFeedbacks.Where(x => accounts.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // Charge -> Bill
            var chargesOfBillsOfAccounts = context.Charges.Where(x => billsOfAccounts.Select(a => a.BillID).ToList().Contains(x.BillID));


            // Remove Charges
            context.Charges.RemoveRange(chargesOfBillsOfAccounts);
            context.Charges.RemoveRange(chargesOfBillsOfAccountsOfOrder);

            // Remove Bills
            context.Bills.RemoveRange(billsOfAccounts);
            context.Bills.RemoveRange(billsOfAccountsOfOrder);

            // Remove CF
            context.CustomerFeedbacks.RemoveRange(cfOfAccounts);
            context.CustomerFeedbacks.RemoveRange(cfOfAccountsOfOrder);

            // Remove Accounts
            context.Accounts.RemoveRange(accounts);
            context.Accounts.RemoveRange(accountsOfOrder);

            // Remove Orders
            context.Orders.RemoveRange(orders);

            // Remove Call Charges
            context.CallCharges.RemoveRange(callCharges);

            context.PaymentPlanDetails.Remove(paymentPlanDetail);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete payment plan detail";
            return Redirect("/Admin/PaymentPlan/Detail?paymentPlanID=" + paymentPlanDetail.PaymentPlanID);
        }
    }
}