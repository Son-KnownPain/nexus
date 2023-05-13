using eProject.Auth;
using eProject.Filters;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    [CustomerAuthorization]
    public class BillController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Bill/List
        public ActionResult List(string accountID)
        {
            if (accountID == null) return Redirect("/");

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == accountID);

            if (account == null || account.CustomerID != AuthManager.CurrentCustomer.GetCustomer.CustomerID) 
                return Redirect("/");

            ViewBag.AccountID = accountID;

            ViewBag.bills = context.Bills.Where(b => b.AccountID == account.AccountID).OrderByDescending(b => b.BillID).ToList();

            return View();
        }
    }
}