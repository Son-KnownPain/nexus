using eProject.Auth;
using eProject.Models;
using eProject.Models.ViewModels.Order;
using eProject.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class OrderController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Order/Prepare
        public ActionResult Prepare(int? serviceID, int? paymentPlanDetailID)
        {
            if (serviceID == null || paymentPlanDetailID == null) return Redirect("/");

            Service service = context.Services.FirstOrDefault(s => s.ServiceID == serviceID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(a => a.PaymentPlanDetailID == paymentPlanDetailID);

            if (service == null || pld == null) return Redirect("/");

            // Check if pld not belong service
            if (context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID).ServiceID != service.ServiceID) 
                return Redirect("/");

            ViewBag.service = service;
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(a => a.PaymentPlanID == pld.PaymentPlanID);

            if (AuthManager.IsCustomerAuthenticated)
            {
                OrderRegisterViewModel viewModel = new OrderRegisterViewModel();
                int CustomerID = AuthManager.CurrentCustomer.GetCustomer.CustomerID;
                Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == CustomerID);
                if (customer == null) return Redirect("/");

                viewModel.Fullname = customer.Fullname;
                viewModel.Email = customer.Email;
                viewModel.Address = customer.Address;
                viewModel.AddressDetail = customer.AddressDetail;
                viewModel.Phone = customer.Phone;

                return View(viewModel);
            }

            return View();
        }

        // Method handle generate next order id
        private string GetOrderID(int serviceID, int paymentPlanDetaiID)
        {
            Order lastOrder = context.Orders.ToList().LastOrDefault();
            Service service = context.Services.FirstOrDefault(s => s.ServiceID == serviceID);
            if (service == null) return null;
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(p => p.PaymentPlanDetailID == paymentPlanDetaiID);
            if (pld == null) return null;
            if (context.PaymentPlans.FirstOrDefault(a => a.PaymentPlanID == pld.PaymentPlanID).ServiceID != serviceID)
            {
                return null;
            }

            string serviceName = service.ServiceName;
            string serviceCharacter;
            switch (serviceName)
            {
                case "Dial-up":
                    serviceCharacter = "D";
                    break;
                case "Broadband":
                    serviceCharacter = "B";
                    break;
                case "Landline":
                    serviceCharacter = "T";
                    break;
                default:
                    serviceCharacter = serviceName[0].ToString();
                    break;
            }

            if (lastOrder == null)
            {
                return serviceCharacter + "0000000001";
            }
            else
            {
                int nextID = Convert.ToInt32(lastOrder.OrderID.Substring(1)) + 1;
                int nextIDLength = nextID.ToString().Length;
                string zeroString = "";
                for (int i = 0; i < 10 - nextIDLength; i++)
                {
                    zeroString += "0";
                }
                return serviceCharacter + zeroString + nextID;
            }
        }

        private double GetDepositDiscount(int connectQuantity)
        {
            ConnectQuantityDiscount discount =
                context.ConnectQuantityDiscounts.FirstOrDefault(a => connectQuantity >= a.QuantityFrom && connectQuantity <= a.QuantityTo);
            if (discount == null) return 0d;
            return discount.DiscountValue;
        }

        // POST: Order/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Store(OrderRegisterViewModel data)
        {
            Service service = context.Services.FirstOrDefault(s => s.ServiceID == data.ServiceID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(a => a.PaymentPlanDetailID == data.PaymentPlanDetailID);

            if (service == null || pld == null) return Redirect("/");

            // Check if pld not belong service
            if (context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID).ServiceID != service.ServiceID)
                return Redirect("/");

            ViewBag.service = service;
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(a => a.PaymentPlanID == pld.PaymentPlanID);

            if (!ModelState.IsValid) return View("Prepare");

            if (context.Customers.FirstOrDefault(c => c.Phone == data.Phone) != null && !AuthManager.IsCustomerAuthenticated)
            {
                ModelState.AddModelError("Phone", "Phone already existing, try other phone");
                return View("Prepare");
            }

            // Handle create new customer
            Customer customer = null;
            if (!AuthManager.IsCustomerAuthenticated)
            {
                customer = new Customer();
                customer.Fullname = data.Fullname;
                customer.Phone = data.Phone;
                customer.Address = data.Address;
                customer.AddressDetail = data.AddressDetail;
                customer.Email = data.Email;
                customer.Avatar = "default-user-avatar.png";
                customer.Password = Crypto.HashPassword(data.Password);
                customer.CreatedAt = DateTime.Now;
                customer.UpdatedAt = DateTime.Now;
            } else
            {
                int CustomerID = AuthManager.CurrentCustomer.GetCustomer.CustomerID;
                customer = context.Customers.FirstOrDefault(c => c.CustomerID == CustomerID);

                if (customer == null) return Redirect("/");
            }

            // Handle create new order
            /**
             * Generate order id
             * 1. Take service character: D, B, T
             * 2. Get previou order id and increase id
             */
            string orderID = GetOrderID(data.ServiceID, data.PaymentPlanDetailID);

            if (orderID == null) return Redirect("/");

            Order order = new Order();
            order.OrderID = orderID;
            order.ServiceID = data.ServiceID;
            if (AuthManager.IsCustomerAuthenticated)
            {
                order.CustomerID = customer.CustomerID;
            }
            order.PaymentPlanDetailID = data.PaymentPlanDetailID;
            order.Status = "Pending";
            order.Phone = data.Phone;
            order.Address = data.Address;
            order.AddressDetail = data.AddressDetail;
            order.OrderDate = DateTime.Now;
            order.Deposit = (int) context.Services.FirstOrDefault(s => s.ServiceID == data.ServiceID).Deposit;
            order.DepositDiscount = GetDepositDiscount(data.ConnectQuantity);
            order.ConnectQuantity = data.ConnectQuantity;

            // Login
            if (!AuthManager.IsCustomerAuthenticated)
            {
                AuthManager.Chef.PreservationCustomerCookies(customer);
            }

            try
            {
                if (customer != null)
                {
                    context.Customers.Add(customer);
                }
                context.Orders.Add(order);
                
                context.SaveChanges();
            }
            catch (Exception e)
            {
                context.Orders.Remove(order);
                if (customer != null)
                {
                    context.Customers.Remove(customer);
                }
                
                return Store(data);
            }

            // Redirect to customer's order
            TempData["Success"] = "Successfully order the service, please check your order";
            return Redirect("/Customer/MyOrders");
        }

        // GET: Order/Detail
        [CustomerAuthorization]
        public ActionResult Detail(string orderID)
        {
            if (orderID == null) return Redirect("/");

            Order order = context.Orders.FirstOrDefault(o => o.OrderID == orderID);

            if (order == null || order.CustomerID != AuthManager.CurrentCustomer.GetCustomer.CustomerID) return Redirect("/");

            ViewBag.service = context.Services.FirstOrDefault(s => s.ServiceID == order.ServiceID);
            PaymentPlanDetail pld = context.PaymentPlanDetails.FirstOrDefault(m => m.PaymentPlanDetailID == order.PaymentPlanDetailID);
            ViewBag.pld = pld;
            ViewBag.pl = context.PaymentPlans.FirstOrDefault(m => m.PaymentPlanID == pld.PaymentPlanID);

            return View(order);
        }

        // GET: Order/Cancel
        [CustomerAuthorization]
        public ActionResult Cancel(string orderID)
        {
            if (orderID == null) return Redirect("/");

            Order order = context.Orders.FirstOrDefault(o => o.OrderID == orderID);

            if (order == null || order.CustomerID != AuthManager.CurrentCustomer.GetCustomer.CustomerID) return Redirect("/");

            if (order.Status.Equals("Pending"))
            {
                order.Status = "Customer has canceled";

                context.SaveChanges();

                TempData["Success"] = "Successfully cancel order";
            }
            else
            {
                TempData["Error"] = "Order cannot cancel";
            }

            return Redirect("/Customer/MyOrders");
        }
    }
}