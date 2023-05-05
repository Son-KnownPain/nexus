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
    }
}