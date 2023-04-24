using eProject.Auth;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class CustomerController : Controller
    {
        private NexusEntities context = new NexusEntities();

        private string GenerateCustomerID()
        {
            string result = "P" + "000" + DateTime.Now.ToString("ddHHmmssffff");

            return result;
        }

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



            AuthManager.CurrentCustomer.Update(customer);
            TempData["Success"] = "Successfully login";
            return Redirect("/");
        }

        // GET: Customer/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Customer/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Store(CustomerRegister data)
        {
            bool isExistingUsername = context.Customers.FirstOrDefault(acc => acc.Phone == data.Phone) != null;

            if (isExistingUsername)
            {
                ModelState.AddModelError("Phone", "Phone existing, try other phone");
            }
            if (!ModelState.IsValid) return View("Register");

            Customer customer = new Customer();
            customer.CustomerID = GenerateCustomerID();
            customer.Phone = data.Phone;
            customer.Fullname = data.Fullname;
            customer.Email = "";
            customer.Password = Crypto.HashPassword(data.Password);
            customer.Address = "";
            customer.AddressDetail = "";
            customer.Avatar = "default-user-avatar.png";
            customer.CreatedAt = DateTime.Now;
            customer.UpdatedAt = DateTime.Now;

            context.Customers.Add(customer);
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            TempData["Success"] = "Successfully account registration";

            return RedirectToAction("Login");
        }
    }
}