using eProject.Models;
using eProject.Models.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using System.IO;
using System.Web.Helpers;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class CustomerController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/Customer
        public ActionResult Index()
        {
            ViewBag.customers = context.Customers.ToList();
            return View();
        }

        // GET: Admin/Customer/Edit
        public ActionResult Edit(int? customerID)
        {
            if (customerID == null) return RedirectToAction("Index");
            Customer customer = context.Customers.FirstOrDefault(e => e.CustomerID == customerID);
            if (customer == null) return RedirectToAction("Index");

            CustomerEditViewModel model = new CustomerEditViewModel();
            model.CustomerID = customer.CustomerID;
            model.Fullname = customer.Fullname;
            model.Phone = customer.Phone;
            model.Email = customer.Email;
            model.Address = customer.Address;
            model.AddressDetail = customer.AddressDetail;
            model.Avatar = customer.Avatar;

            return View(model);
        }

        // PUT: Admin/Customer/Update
        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult Update(CustomerEditViewModel data, HttpPostedFileBase AvatarFile)
        {
            if (!ModelState.IsValid) return View("Edit");

            Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == data.CustomerID);

            if (customer == null) return RedirectToAction("Index");

            customer.CustomerID = data.CustomerID;
            customer.Fullname = data.Fullname;
            customer.Phone = data.Phone;
            customer.Email = data.Email;
            customer.Address = data.Address;
            customer.AddressDetail = data.AddressDetail;

            if (AvatarFile != null && AvatarFile.ContentLength > 0)
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
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            TempData["Success"] = "Successfully update customer information";

            return RedirectToAction("Index");
        }

        // GET: Admin/Customer/ChangePassword
        public ActionResult ChangePassword(int? customerID)
        {
            if (customerID == null) return RedirectToAction("Index");
            Customer customer = context.Customers.FirstOrDefault(e => e.CustomerID == customerID);
            if (customer == null) return RedirectToAction("Index");

            ViewBag.CustomerName = customer.Fullname;

            CustomerPasswordEditViewModel model = new CustomerPasswordEditViewModel();

            model.CustomerID = customer.CustomerID;

            return View(model);
        }

        // PUT: Admin/Customer/UpdatePassword
        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(CustomerPasswordEditViewModel data)
        {
            if (!ModelState.IsValid) return View("ChangePassword");

            try
            {
                Customer customer = context.Customers.FirstOrDefault(e => e.CustomerID == data.CustomerID);
                if (customer == null) return RedirectToAction("Index");

                customer.Password = Crypto.HashPassword(data.NewPassword);
                customer.UpdatedAt = DateTime.Now;

                context.SaveChanges();

                TempData["Success"] = "Successfully update customer password";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: Admin/Customer/Search
        public ActionResult Search(string keyword)
        {
            ViewBag.customers = context.Customers
                .Where(e =>
                    e.Phone.Contains(keyword) ||
                    e.Fullname.Contains(keyword) ||
                    e.Email.Contains(keyword) ||
                    e.Address.Contains(keyword)
                )
                .ToList();
            return View("Index");
        }
    }
}