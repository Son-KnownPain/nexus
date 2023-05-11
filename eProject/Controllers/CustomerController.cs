using eProject.Auth;
using eProject.Filters;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using eProject.Models.ViewModels.Customer;
using eProject.Models.ViewModels.Order;
using System.Security.Policy;
using eProject.Models.ViewModels.Account;

namespace eProject.Controllers
{
    public class CustomerController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Customer/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Customer/Check
        public ActionResult Check(SimpleProxyCustomer data)
        {
            if (!ModelState.IsValid) return View("Login");
            Customer customer = context.Customers.FirstOrDefault(acc => acc.Phone == data.Phone);

            if (customer == null)
            {
                TempData["Error"] = "Phone or password incorrect";
                return RedirectToAction("Login");
            } 
            else if (!Crypto.VerifyHashedPassword(customer.Password, data.Password))
            {
                TempData["Error"] = "Phone or password incorrect";
                return RedirectToAction("Login");
            }


            AuthManager.Chef.PreservationCustomerCookies(customer);
            TempData["Success"] = "Successfully login";
            return RedirectToAction("Profile");
        }

        // GET: Customer/SignOut
        [CustomerAuthorization]
        public ActionResult SignOut()
        {
            AuthManager.Chef.SpoiledCustomer();
            TempData["Success"] = "Successfully sign out";
            return RedirectToAction("Login");
        }

        // GET: Customer/Profile
        [CustomerAuthorization]
        public new ActionResult Profile()
        {
            Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == AuthManager.CurrentCustomer.GetCustomer.CustomerID);
            if (customer == null) return Redirect("/");
            return View(customer);
        }

        // PUT: Customer/ChangeAvatar
        [HttpPut]
        [ValidateAntiForgeryToken]
        [CustomerAuthorization]
        public ActionResult ChangeAvatar(HttpPostedFileBase AvatarFile)
        {
            int id = AuthManager.CurrentCustomer.GetCustomer.CustomerID;
            Customer customer = context.Customers.FirstOrDefault(e => e.CustomerID == id);

            if (AvatarFile != null && AvatarFile.ContentLength > 0 && customer != null)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                String uploadFolderPath = Server.MapPath("~/Uploads/CustomerAvatars");

                String extName = Path.GetExtension(AvatarFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "customer-avatar" + randomNumber + extName;

                AvatarFile.SaveAs(uploadFolderPath + "/" + newFileName);

                // Delete old file
                if (System.IO.File.Exists(uploadFolderPath + "/" + customer.Avatar) && !customer.Avatar.Equals("default-user-avatar.png"))
                {
                    System.IO.File.Delete(uploadFolderPath + "/" + customer.Avatar);
                }

                customer.Avatar = newFileName;
                customer.UpdatedAt = DateTime.Now;

                context.SaveChanges();

                TempData["Success"] = "Successfully update avatar";
            }

            return RedirectToAction("Profile");
        }

        // GET: Customer/EditPassword
        [CustomerAuthorization]
        public ActionResult EditPassword()
        {
            return View();
        }

        // PUT: Customer/UpdatePassword
        [HttpPut]
        [ValidateAntiForgeryToken]
        [CustomerAuthorization]
        public ActionResult UpdatePassword(MyPasswordEdit data)
        {
            if (!ModelState.IsValid) return View("EditPassword");

            int id = AuthManager.CurrentCustomer.GetCustomer.CustomerID;
            Customer customer = context.Customers.FirstOrDefault(e => e.CustomerID == id);
            if (customer != null && Crypto.VerifyHashedPassword(customer.Password, data.CurrentPassword))
            {
                customer.Password = Crypto.HashPassword(data.NewPassword);
                customer.UpdatedAt = DateTime.Now;

                context.SaveChanges();

                TempData["Success"] = "Successfully update my password";

                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError("CurrentPassword", "Current password incorrect, type again!");
                return View("EditPassword");
            }
        }

        // GET: Customer/EditInformation
        [CustomerAuthorization]
        public ActionResult EditInformation()
        {
            int id = AuthManager.CurrentCustomer.GetCustomer.CustomerID;
            Customer customer = context.Customers.FirstOrDefault(e => e.CustomerID == id);
            if (customer == null) return Redirect("/");

            CustomerChangeInfoViewModel model = new CustomerChangeInfoViewModel();

            model.CustomerID = customer.CustomerID;
            model.Fullname = customer.Fullname;
            model.Phone = customer.Phone;
            model.Email = customer.Email;
            model.Address = customer.Address;
            model.AddressDetail = customer.AddressDetail;

            return View(model);
        }

        // PUT: Customer/UpdateInformation
        [HttpPut, ValidateAntiForgeryToken]
        [CustomerAuthorization]
        public ActionResult UpdateInformation(CustomerChangeInfoViewModel data)
        {
            Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == data.CustomerID);

            customer.CustomerID = data.CustomerID;
            customer.Fullname = data.Fullname;
            customer.Phone = data.Phone;
            customer.Email = data.Email;
            customer.Address = data.Address;
            customer.AddressDetail = data.AddressDetail;
            customer.UpdatedAt = DateTime.Now;

            context.SaveChanges();

            TempData["Success"] = "Successfully update information";

            return RedirectToAction("Profile");
        }

        // GET: Customer/MyOrders
        [CustomerAuthorization]
        public ActionResult MyOrders()
        {
            int customerID = AuthManager.CurrentCustomer.GetCustomer.CustomerID;
            ViewBag.orders = context.Orders
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
                ).Where(o => o.CustomerID == customerID).ToList();
            return View();
        }

        // GET: Admin/Customer/MyConnections
        [CustomerAuthorization]
        public ActionResult MyConnections()
        {
            int customerID = AuthManager.CurrentCustomer.GetCustomer.CustomerID;
            ViewBag.accounts = context.Accounts
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
                ).Where(c => c.CustomerID == customerID).ToList();

            return View();
        }
    }
}