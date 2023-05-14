using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Models;
using eProject.Models.ViewModels.PaymentPlans;
using eProject.Filters;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class PaymentPlanController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/PaymentPlan
        public ActionResult Index()
        {
            ViewBag.paymentPlans = context.PaymentPlans.Join(context.Services, p => p.ServiceID, s => s.ServiceID, (p, s) => new PaymentPlanViewModel
            {
                PaymentPlanID = p.PaymentPlanID,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                PlanName = p.PlanName,
                Description = p.Description
            }).ToList();
            return View();
        }

        // GET: Admin/PaymentPlan/Add
        public ActionResult Add()
        {
            return View();
        }

        // POST: Admin/PaymentPlan/Store
        [HttpPost]
        [ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Store(PaymentPlanViewModel data)
        {
            if (!ModelState.IsValid) return View("Add");

            if (context.Services.FirstOrDefault(s => s.ServiceID == data.ServiceID) == null)
            {
                ModelState.AddModelError("ServiceID", "Service not found");
                return View("Add");
            }

            PaymentPlan paymentPlan = new PaymentPlan();
            paymentPlan.ServiceID = data.ServiceID;
            paymentPlan.PlanName = data.PlanName;
            paymentPlan.Description = data.Description;

            context.PaymentPlans.Add(paymentPlan);

            context.SaveChanges();

            TempData["Success"] = "Successfully add new payment plan for a service";

            return RedirectToAction("Index");
        }

        // GET: Admin/PaymentPlan/Detail
        public ActionResult Detail(int? paymentPlanID)
        {
            if (paymentPlanID == null) return RedirectToAction("Index");

            PaymentPlanViewModel viewModel = context.PaymentPlans.Join(context.Services, p => p.ServiceID, s => s.ServiceID, (p, s) => new PaymentPlanViewModel
            {
                PaymentPlanID = p.PaymentPlanID,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                PlanName = p.PlanName,
                Description = p.Description
            }).FirstOrDefault(m => m.PaymentPlanID == paymentPlanID);

            if (viewModel == null) return RedirectToAction("Index");

            ViewBag.detailItems = context.PaymentPlanDetails
                .Where(m => m.PaymentPlanID == paymentPlanID).ToList();

            return View(viewModel);
        }

        // GET: Admin/PaymentPlan/Edit
        public ActionResult Edit(int? paymentPlanID)
        {
            if (paymentPlanID == null) return RedirectToAction("Index");

            PaymentPlanViewModel viewModel = context.PaymentPlans.Join(context.Services, p => p.ServiceID, s => s.ServiceID, (p, s) => new PaymentPlanViewModel
            {
                PaymentPlanID = p.PaymentPlanID,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                PlanName = p.PlanName,
                Description = p.Description
            }).FirstOrDefault(m => m.PaymentPlanID == paymentPlanID);

            if (viewModel == null) return RedirectToAction("Index");

            return View(viewModel);
        }

        // PUT: Admin/PaymentPlan/Update
        [HttpPut]
        [ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(PaymentPlanViewModel data)
        {
            if (!ModelState.IsValid) return View("Add");

            if (context.Services.FirstOrDefault(s => s.ServiceID == data.ServiceID) == null)
            {
                ModelState.AddModelError("ServiceID", "Service not found");
                return View("Add");
            }

            PaymentPlan paymentPlan = context.PaymentPlans.FirstOrDefault(p => p.PaymentPlanID == data.PaymentPlanID);
            if (paymentPlan == null) RedirectToAction("Index");
            paymentPlan.ServiceID = data.ServiceID;
            paymentPlan.PlanName = data.PlanName;
            paymentPlan.Description = data.Description;

            context.SaveChanges();

            TempData["Success"] = "Successfully update payment plan for a service";

            return RedirectToAction("Index");
        }

        // GET: Admin/PaymentPlan/Search
        public ActionResult Search(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.paymentPlans = context.PaymentPlans.Join(context.Services, p => p.ServiceID, s => s.ServiceID, (p, s) => new PaymentPlanViewModel
            {
                PaymentPlanID = p.PaymentPlanID,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                PlanName = p.PlanName,
                Description = p.Description
            }).Where(m => m.ServiceName.Contains(keyword) || m.PlanName.Contains(keyword)).ToList();
            }
            else
            {
                TempData["Error"] = "No keyword entered, please enter your keyword";
                return RedirectToAction("Index");
            }
            
            return View("Index");
        }

        public ActionResult Delete(int? paymentPlanID)
        {
            if (paymentPlanID == null)
            {
                return RedirectToAction("Index");
            }

            PaymentPlan paymentPlan = context.PaymentPlans.FirstOrDefault(p => p.PaymentPlanID == paymentPlanID);
            if (paymentPlan == null)
            {
                return RedirectToAction("Index");
            }


            // PLD -> PL
            var plds = context.PaymentPlanDetails.Where(x => x.PaymentPlanID == paymentPlan.PaymentPlanID);

            // Call charge -> PLD
            var callCharges = context.CallCharges.Where(x => plds.Select(s => s.PaymentPlanDetailID).ToList().Contains(x.PaymentPlanDetailID));

            // ------------------------------------------------------------

            // Order -> PLD
            var orders = context.Orders.Where(x => plds.Select(s => s.PaymentPlanDetailID).ToList().Contains(x.PaymentPlanDetailID));
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
            var accounts = context.Accounts.Where(x => plds.Select(s => s.PaymentPlanDetailID).ToList().Contains(x.PaymentPlanDetailID));
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

            // Remove Orderes
            context.Orders.RemoveRange(orders);

            // Remove Call Charges
            context.CallCharges.RemoveRange(callCharges);

            // Remove Payment Plan Detail
            context.PaymentPlanDetails.RemoveRange(plds);

            // Remove Payment Plan
            context.PaymentPlans.Remove(paymentPlan);

            context.SaveChanges();

            TempData["Success"] = "Successfully delete payment plan";
            return RedirectToAction("Index"); 
        }
    }
}