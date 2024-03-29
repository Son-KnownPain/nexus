﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Auth;
using eProject.Filters;
using eProject.Models;
using eProject.Models.ViewModels.Bill;

namespace eProject.Areas.Admin.Controllers
{
    [EmployeeAuthorization]
    public class BillController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/Bill/Index
        public ActionResult Index(string accountID)
        {
            List<Bill> listBill;
            if (accountID != null)
            {
                listBill = context.Bills.Where(b => b.AccountID == accountID)
                    .OrderByDescending(b => b.BillID)
                    .ToList();
            } else
            {
                listBill = context.Bills.OrderByDescending(b => b.BillID).ToList();
            }
            ViewBag.bills = listBill;
            return View();
        }

        // GET: Admin/Bill/Generate
        public ActionResult Generate(string accountID)
        {
            if (accountID == null) return RedirectToAction("Index");

            Account account = context.Accounts.FirstOrDefault(a => a.AccountID == accountID);

            if (account == null) return RedirectToAction("Index");

            int eid = AuthManager.CurrentEmployee.GetEmployee.EmployeeID;

            Employee emp = context.Employees.FirstOrDefault(e => e.EmployeeID == eid);

            if (emp != null)
            {
                ViewBag.EmployeeName = emp.Fullname;
            }

            ViewBag.account = account;
            ViewBag.service = context.Services.FirstOrDefault(s => s.ServiceID == account.ServiceID);
            ViewBag.customer = context.Customers.FirstOrDefault(c => c.CustomerID == account.CustomerID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == account.PaymentPlanDetailID);
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID);

            ConnectQuantityDiscount cqd = context.ConnectQuantityDiscounts.FirstOrDefault(a => a.QuantityFrom <= account.ConnectQuantity && account.ConnectQuantity <= a.QuantityTo);

            Bill bill = new Bill();

            bill.AccountID = accountID;
            bill.InitialDueAmount = pld.RentalCost * account.ConnectQuantity;
            bill.Discount =  cqd != null ? cqd.DiscountValue : 0;
            bill.VatCost = (bill.InitialDueAmount - (bill.InitialDueAmount * bill.Discount)) * 0.1224;
            bill.AmountPaid = bill.InitialDueAmount - (bill.InitialDueAmount * bill.Discount) + bill.VatCost;
            bill.PaidContent = "This is bill for your service!";

            return View(bill);
        }

        // POST: Admin/Bill/Store
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Store(Bill data)
        {
            if (!ModelState.IsValid)
            {
                if (data.AccountID == null) return RedirectToAction("Index");

                Account account = context.Accounts.FirstOrDefault(a => a.AccountID == data.AccountID);

                if (account == null) return RedirectToAction("Index");

                int empID = AuthManager.CurrentEmployee.GetEmployee.EmployeeID;

                Employee emp = context.Employees.FirstOrDefault(e => e.EmployeeID == empID);

                if (emp != null)
                {
                    ViewBag.EmployeeName = emp.Fullname;
                }

                ViewBag.account = account;
                ViewBag.service = context.Services.FirstOrDefault(s => s.ServiceID == account.ServiceID);
                ViewBag.customer = context.Customers.FirstOrDefault(c => c.CustomerID == account.CustomerID);
                PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == account.PaymentPlanDetailID);
                ViewBag.pld = pld;
                ViewBag.pl = context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID);

                return View("Generate");
            }

            Bill bill = new Bill();

            int eid = AuthManager.CurrentEmployee.GetEmployee.EmployeeID;

            bill.AccountID = data.AccountID;
            bill.EmployeeID = eid;
            bill.InitialDueAmount = data.InitialDueAmount;
            bill.Discount = data.Discount;
            bill.VatCost = data.VatCost;
            bill.AmountPaid = data.AmountPaid;
            bill.PaidContent = data.PaidContent;
            bill.CreatedDate = DateTime.Now;

            if (data.Paid)
            {
                bill.Status = "Paid";
            } else
            {
                bill.Status = "Unpaid";
            }

            context.Bills.Add(bill);

            context.SaveChanges();

            TempData["Success"] = "Successfully add new bill";

            return RedirectToAction("Index");
        }

        // GET: Admin/Bill/Edit
        public ActionResult Edit(int? billID)
        {
            if (billID == null) return RedirectToAction("Index");

            Bill bill = context.Bills.FirstOrDefault(b => b.BillID == billID);

            if (bill == null) return RedirectToAction("Index");

            ViewBag.charges = context.Charges.Where(c => c.BillID == bill.BillID).ToList();

            BillEditModel model = new BillEditModel();

            model.BillID = bill.BillID;
            model.InitialDueAmount = bill.InitialDueAmount;
            model.Discount = bill.Discount;
            model.VatCost = bill.VatCost;
            model.AmountPaid = bill.AmountPaid;
            model.PaidContent = bill.PaidContent;
            model.Paid = bill.Status.Equals("Paid");

            return View(bill);
        }


        // GET: Admin/Bill/Delete
        public ActionResult Delete(int? billID)
        {
            if (billID == null)
            {
                return RedirectToAction("Index");
            }

            Bill bill = context.Bills.FirstOrDefault(b => b.BillID == billID);
            if (bill == null)
            {
                return RedirectToAction("Index");
            }

            
            context.Charges.RemoveRange(context.Charges.Where(c => c.BillID == billID));

            context.Bills.Remove(bill);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete bill";
            return RedirectToAction("Index");
        }

        // PUT: Admin/Bill/Update
        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult Update(BillEditModel data)
        {
            if (!ModelState.IsValid) return View("Edit");

            Bill bill = context.Bills.FirstOrDefault(a => a.BillID == data.BillID);

            if (bill == null) return RedirectToAction("Index");

            bill.InitialDueAmount = data.InitialDueAmount;
            bill.Discount = data.Discount;
            bill.VatCost = data.VatCost;
            bill.AmountPaid = data.AmountPaid;
            bill.PaidContent = data.PaidContent;

            if (data.Paid)
            {
                bill.Status = "Paid";
            }
            else
            {
                bill.Status = "Unpaid";
            }

            context.SaveChanges();

            TempData["Success"] = "Successfully update bill";

            return RedirectToAction("Index");
        }

        // POST: Admin/Bill/Search
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Search(string keyword, string status)
        {
            switch(status)
            {
                case "Paid":
                    ViewBag.bills = context.Bills.Where(b => b.AccountID.Contains(keyword) && b.Status.Equals("Paid")).OrderByDescending(b => b.BillID).ToList();
                    break;
                case "Unpaid":
                    ViewBag.bills = context.Bills.Where(b => b.AccountID.Contains(keyword) && b.Status.Equals("Unpaid")).OrderByDescending(b => b.BillID).ToList();
                    break;
                default:
                    ViewBag.bills = context.Bills.Where(b => b.AccountID.Contains(keyword)).OrderByDescending(b => b.BillID).ToList();
                    break;
            }
            
            return View("Index");
        }
    }
}