﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using eProject.Models.ViewModels.Order;
using eProject.Models;
using eProject.Auth;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class OrderController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/Order
        public ActionResult Index()
        {
            List<OrderViewModel> listOrder = context.Orders
                .Join(context.PaymentPlanDetails, o => o.PaymentPlanDetailID, pld => pld.PaymentPlanDetailID, (o, pld) => new
                {
                    o,
                    pld
                })
                .Join(context.Services, x => x.o.ServiceID, s => s.ServiceID, (x, s) =>
                    new OrderViewModel
                    {
                        CustomerID = x.o.CustomerID,
                        PaymentPlanDetailID = x.pld.PaymentPlanDetailID,
                        Content = x.pld.Content,
                        ServiceName = s.ServiceName,
                        Thumbnail = s.Thumbnail,
                        OrderID = x.o.OrderID,
                        Status = x.o.Status,
                        Phone = x.o.Phone,
                        Address = x.o.Address,
                        AddressDetail = x.o.AddressDetail,
                        ConnectQuantity = x.o.ConnectQuantity,
                        Deposit = x.o.Deposit,
                        DepositDiscount = x.o.DepositDiscount,
                        OrderDate = x.o.OrderDate
                    }
                ).ToList();
            listOrder.Reverse();
            ViewBag.orders = listOrder;
            return View();
        }

        // GET: Admin/Order/Detail
        public ActionResult Detail(string orderID)
        {
            if (orderID == null) return Redirect("/");

            Order order = context.Orders.FirstOrDefault(o => o.OrderID == orderID);

            if (order == null) return Redirect("/");

            ViewBag.service = context.Services.FirstOrDefault(s => s.ServiceID == order.ServiceID);
            ViewBag.customer = context.Customers.FirstOrDefault(c => c.CustomerID == order.CustomerID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == order.PaymentPlanDetailID);
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID);

            return View(order);
        }

        // GET: Admin/Order/ChangeStatus
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(Order data)
        {
            Order order = context.Orders.FirstOrDefault(o => o.OrderID == data.OrderID);

            if (order == null) return RedirectToAction("Index");

            string completeStatus = "The order has been completed";

            if (data.Status.Equals(completeStatus)) return RedirectToAction("CompleteOrder", new { orderID = data.OrderID });

            //if (!order.Status.Equals(completeStatus))
            //{
            order.Status = data.Status;
            //}
            //else
            //{
            //    TempData["Error"] = "The order was completed, cannot change order status";

            //    return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/Admin/Order");
            //}

            context.SaveChanges();
            TempData["Success"] = "Successfully change order status";

            return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/Admin/Order");
        }

        // GET: Admin/Order/CompleteOrder
        public ActionResult CompleteOrder(string orderID)
        {
            if (orderID == null) return RedirectToAction("Index");
            Order order = context.Orders.FirstOrDefault(o => o.OrderID == orderID);
            if (order == null) return RedirectToAction("Index");

            OrderCompleteViewModel model = new OrderCompleteViewModel();

            model.OrderID = order.OrderID;

            return View(model);
        }

        // PUT: Admin/Order/Complete
        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult Complete(OrderCompleteViewModel data)
        {
            if (!ModelState.IsValid) return View("CompleteOrder");

            Order order = context.Orders.FirstOrDefault(o => o.OrderID == data.OrderID);

            if (order == null) return RedirectToAction("Index");

            order.Status = "The order has been completed";

            Account lastAccount = context.Accounts.ToList().LastOrDefault();

            string accountID;
            if (lastAccount == null)
            {
                accountID = order.OrderID[0] + data.CityCode + "000000000001";
            }
            else
            {
                string lastID = lastAccount.AccountID.Substring(4);
                int lastIDInt = Convert.ToInt32(lastID);
                int newID = lastIDInt + 1;
                string zeroString = "";

                for (int i = 0; i < 12 - newID.ToString().Length; i++)
                {
                    zeroString += "0";
                }

                accountID = order.OrderID[0] + data.CityCode + zeroString + newID;
            }
            

            Account account = new Account();
            account.AccountID = accountID;
            
            account.ServiceID = order.ServiceID;
            account.CustomerID = order.CustomerID;
            account.PaymentPlanDetailID = order.PaymentPlanDetailID;
            account.OrderID = order.OrderID;

            account.ConnectQuantity = order.ConnectQuantity;
            account.Status = "Connecting";
            account.ContactNumber = order.Phone;
            account.DueDate = DateTime.Now.AddDays(context.PaymentPlanDetails.FirstOrDefault(pld => pld.PaymentPlanDetailID == order.PaymentPlanDetailID).VadilityDays);
            account.ConnectedAt = DateTime.Now;

            context.Accounts.Add(account);

            context.SaveChanges();

            TempData["Success"] = "Successfully to complete order";

            return RedirectToAction("Detail", new { orderID = data.OrderID });
        }
    }
}