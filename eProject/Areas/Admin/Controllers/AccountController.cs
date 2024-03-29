﻿using eProject.Models;
using eProject.Models.ViewModels.Account;
using eProject.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Protocols;

namespace eProject.Areas.Admin.Controllers
{
    [EmployeeAuthorization]
    public class AccountController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/Account
        public ActionResult Index()
        {
            List<AccountViewModel> listAccount = context.Accounts
                .Join(context.Customers, a => a.CustomerID, c => c.CustomerID, (a, c) => new
                {
                    a,
                    c
                })
                .Join(context.Services, x => x.a.ServiceID, s => s.ServiceID, (x, s) =>
                    new AccountViewModel
                    {
                        AccountID = x.a.AccountID,
                        CustomerID = x.a.CustomerID,
                        CustomerName = x.c.Fullname,
                        PaymentPlanDetailID = x.a.PaymentPlanDetailID,
                        ServiceName = s.ServiceName,
                        Thumbnail = s.Thumbnail,
                        OrderID = x.a.OrderID,
                        Status = x.a.Status,
                        ContactNumber = x.a.ContactNumber,
                        ConnectQuantity = x.a.ConnectQuantity,
                        DueDate = x.a.DueDate,
                        ConnectedAt = x.a.ConnectedAt,
                    }
                ).ToList();
            listAccount.Reverse();
            ViewBag.accounts = listAccount;
            return View();
        }

        // GET: Admin/Account/Detail
        public ActionResult Detail(string accountID)
        {
            if (accountID == null) return RedirectToAction("Index");

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == accountID);

            if (account == null) return RedirectToAction("Index");

            ViewBag.service = context.Services.FirstOrDefault(s => s.ServiceID == account.ServiceID);
            ViewBag.customer = context.Customers.FirstOrDefault(c => c.CustomerID == account.CustomerID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == account.PaymentPlanDetailID);
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID);

            return View(account);
        }

        // GET: Admin/Account/ToggleStatus
        public ActionResult ToggleStatus(string accountID)
        {
            if (accountID == null) return RedirectToAction("Index");

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == accountID);

            if (account == null) return RedirectToAction("Index");

            if (account.Status.Equals("Connecting"))
            {
                account.Status = "Disconnect";
            } else
            {
                account.Status = "Connecting";
            }

            context.SaveChanges();

            return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/Account/Detail?accountID=" + account.AccountID);
        }

        // GET: Admin/Account/Search
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Search(string keyword, string filter)
        {
            List<AccountViewModel> listAccount = context.Accounts
                .Join(context.Customers, a => a.CustomerID, c => c.CustomerID, (a, c) => new
                {
                    a,
                    c
                })
                .Join(context.Services, x => x.a.ServiceID, s => s.ServiceID, (x, s) =>
                    new AccountViewModel
                    {
                        AccountID = x.a.AccountID,
                        CustomerID = x.a.CustomerID,
                        CustomerName = x.c.Fullname,
                        PaymentPlanDetailID = x.a.PaymentPlanDetailID,
                        ServiceName = s.ServiceName,
                        Thumbnail = s.Thumbnail,
                        OrderID = x.a.OrderID,
                        Status = x.a.Status,
                        ContactNumber = x.a.ContactNumber,
                        ConnectQuantity = x.a.ConnectQuantity,
                        DueDate = x.a.DueDate,
                        ConnectedAt = x.a.ConnectedAt,
                    }
                ).Where(x => x.ContactNumber.Contains(keyword) || x.CustomerName.Contains(keyword) || x.AccountID.Equals(keyword)).ToList();

            switch (filter)
            {
                case "Connecting":
                    listAccount = listAccount.Where(a => a.Status.Equals("Connecting")).ToList();
                    break;
                case "Disconnect":
                    listAccount = listAccount.Where(a => a.Status.Equals("Disconnect")).ToList();
                    break;
                case "DueToday":
                    listAccount = listAccount.Where(a => a.DueDate.Date == DateTime.Now.Date).ToList();
                    break;
                default:
                    // Do nothing
                    break;
            }

            listAccount.Reverse();
            ViewBag.accounts = listAccount;
            return View("Index");
        }

        // GET: Admin/Order/Delete
        public ActionResult Delete(string accountID)
        {
            if (accountID == null)
            {
                return RedirectToAction("Index");
            }

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == accountID);

            if (account == null)
            {
                return RedirectToAction("Index");
            }

            // Bills -> Accounts
            var bills = context.Bills.Where(x => x.AccountID == account.AccountID);
            // CF -> Accounts
            var cfs = context.CustomerFeedbacks.Where(x => x.AccountID == account.AccountID);
            // Charges -> Bills
            var chargesOfBills = context.Charges.Where(x => bills.Select(a => a.BillID).ToList().Contains(x.BillID));

            // Remove Charges
            context.Charges.RemoveRange(chargesOfBills);

            // Remove CF
            context.CustomerFeedbacks.RemoveRange(cfs);

            // Remove Bills
            context.Bills.RemoveRange(bills);

            context.Accounts.Remove(account);
            context.SaveChanges();

            TempData["Success"] = "Successfully to delete account";

            return RedirectToAction("Index");
        }
    }
}