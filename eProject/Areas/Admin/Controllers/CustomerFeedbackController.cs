using eProject.Auth;
using eProject.Models;
using eProject.Models.ViewModels.CustomerFeedback;
using eProject.Models.ViewModels.PaymentPlans;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Areas.Admin.Controllers
{
    public class CustomerFeedbackController : Controller
    {
        NexusEntities context = new NexusEntities();

        // GET: Admin/CustomerFeedback
        public ActionResult Index()
        {
            ViewBag.feedbackList = (from c in context.CustomerFeedbacks join e in context.Employees on c.EmployeeID equals e.EmployeeID select new CustomerFeedbackViewModel
            {
                CustomerFeedbackID = c.CustomerFeedbackID,
                EmployeeID = e.EmployeeID,
                AccountID = c.AccountID,
                Content = c.Content,
                ReplyContent = c.ReplyContent,
                FeedbackAt = c.FeedbackAt,
                Avatar = e.Avatar,
                Fullname = e.Fullname,
            }).ToList();
            return View();
        }

        public ActionResult Reply(int? customerFeedbackID)
        {
            if (customerFeedbackID == null) return RedirectToAction("Index");

            CustomerFeedbackViewModel viewModel = context.CustomerFeedbacks.Join(context.Employees, c => c.EmployeeID, e => e.EmployeeID, (c, e) => new CustomerFeedbackViewModel
            {
                CustomerFeedbackID = c.CustomerFeedbackID,
                ReplyContent = c.ReplyContent,
            }).FirstOrDefault(f => f.CustomerFeedbackID == customerFeedbackID);

            if (viewModel == null) return RedirectToAction("Index");

            return View(viewModel);
        }

        [ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ReplyStore(CustomerFeedbackViewModel customerFeedback)
        {

            if (!ModelState.IsValid) return View("Reply");

            CustomerFeedback feedback = context.CustomerFeedbacks.FirstOrDefault(c => c.CustomerFeedbackID == customerFeedback.CustomerFeedbackID);
            if (feedback == null) return RedirectToAction("Index");

            feedback.ReplyContent = customerFeedback.ReplyContent;
            int employeeId = AuthManager.CurrentEmployee.GetEmployee.EmployeeID;
            feedback.EmployeeID = employeeId;

            context.SaveChanges();

            TempData["Success"] = "Successfully reply";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if(id == null) return RedirectToAction("Index");

            CustomerFeedback customerFeedback = context.CustomerFeedbacks.FirstOrDefault(f => f.CustomerFeedbackID == id);
            if (customerFeedback == null) RedirectToAction("Index");

            context.CustomerFeedbacks.Remove(customerFeedback);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete customer feedback";
            return RedirectToAction("Index");
        }

        public ActionResult Search(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.feedbackList = (from c in context.CustomerFeedbacks join e in context.Employees on c.EmployeeID equals e.EmployeeID select new CustomerFeedbackViewModel
                    {
                        CustomerFeedbackID = c.CustomerFeedbackID,
                        EmployeeID = e.EmployeeID,
                        AccountID = c.AccountID,
                        Content = c.Content,
                        ReplyContent = c.ReplyContent,
                        FeedbackAt = c.FeedbackAt,
                        Avatar = e.Avatar,
                        Fullname = e.Fullname,
                    }).Where(c=>c.Content.Contains(keyword)).ToList();
            }
            else
            {
                TempData["Error"] = "No keyword entered, please enter your keyword";
                return RedirectToAction("Index");
            }

            return View("Index");
        }
    }
}