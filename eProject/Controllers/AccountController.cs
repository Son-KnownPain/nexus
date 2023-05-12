using eProject.Auth;
using eProject.Filters;
using eProject.Models;
using eProject.Models.ViewModels.CustomerFeedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    [CustomerAuthorization]
    public class AccountController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Account/Detail
        public ActionResult Detail(string accountID)
        {
            if (accountID == null) return RedirectToAction("Index");

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == accountID);

            if (account == null || account.CustomerID != AuthManager.CurrentCustomer.GetCustomer.CustomerID) return RedirectToAction("Index");

            ViewBag.service = context.Services.FirstOrDefault(s => s.ServiceID == account.ServiceID);
            ViewBag.customer = context.Customers.FirstOrDefault(c => c.CustomerID == account.CustomerID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == account.PaymentPlanDetailID);
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID);

            return View(account);
        }

        // GET: Account/Feedback
        public ActionResult Feedback(string accountID)
        {
            if (accountID == null) return Redirect("/");

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == accountID);

            if (account == null) return Redirect("/");

            CustomerFeedbackViewModel model = new CustomerFeedbackViewModel();

            model.AccountID = account.AccountID;

            ViewBag.feedbacks = context.CustomerFeedbacks.OrderByDescending(f => f.CustomerFeedbackID).Where(f => f.AccountID == account.AccountID).ToList();
            
            return View(model);
        }

        // POST: Account/Feedback
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FeedbackStore(CustomerFeedbackViewModel data)
        {
            if (!ModelState.IsValid) return View("Feedback");

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == data.AccountID);

            if (account == null || account.CustomerID != AuthManager.CurrentCustomer.GetCustomer.CustomerID) return Redirect("/");

            CustomerFeedback cf = new CustomerFeedback();

            cf.AccountID = data.AccountID;
            cf.Content = data.Content;
            cf.ReplyContent = "";
            cf.EmployeeID = null;
            cf.FeedbackAt = DateTime.Now;

            context.CustomerFeedbacks.Add(cf);

            context.SaveChanges();

            TempData["Success"] = "Successfully to feedback";

            return Redirect("/Account/Feedback?accountID=" + data.AccountID);
        }
    }
}