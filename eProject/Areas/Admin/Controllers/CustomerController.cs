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
        public ActionResult Index(int? customerID)
        {
            List<Customer> listCustomer;

            if (customerID != null)
            {
                listCustomer = context.Customers.Where(c => c.CustomerID == customerID)
                    .OrderByDescending(c => c.CustomerID)
                    .ToList();
            } else
            {
                listCustomer = context.Customers.OrderByDescending(c => c.CustomerID).ToList();
            }

            ViewBag.customers = listCustomer;
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

        // GET: Admin/Customer/Delete
        public ActionResult Delete(int? customerID)
        {
            if (customerID == null) return RedirectToAction("Index");

            Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == customerID);

            if (customer == null) return RedirectToAction("Index");

            // Order -> Customer
            var orders = context.Orders.Where(x => x.CustomerID == customer.CustomerID);
            // Account -> Order
            var accountsOfOrder = context.Accounts.Where(x => orders.Select(o => o.OrderID).ToList().Contains(x.OrderID));
            // Bill -> Account
            var billsOfAccountsOfOrder = context.Bills.Where(x => accountsOfOrder.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // CustomerFeedback -> Account
            var cfOfAccountsOfOrder = context.CustomerFeedbacks.Where(x => accountsOfOrder.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // Charge -> Bill
            var chargesOfBillsOfAccountsOfOrder = context.Charges.Where(x => billsOfAccountsOfOrder.Select(a => a.BillID).ToList().Contains(x.BillID));

            // ------------------------------------------------------------

            // Account -> Customer
            var accounts = context.Accounts.Where(x => x.CustomerID == customer.CustomerID);
            // Bill -> Account
            var billsOfAccounts = context.Bills.Where(x => accounts.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // CustomerFeedback -> Account
            var cfOfAccounts = context.CustomerFeedbacks.Where(x => accounts.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // Charge -> Bill
            var chargesOfBillsOfAccounts = context.Charges.Where(x => billsOfAccounts.Select(a => a.BillID).ToList().Contains(x.BillID));


            // Remove Charges
            context.Charges.RemoveRange(chargesOfBillsOfAccounts);
            context.Charges.RemoveRange(chargesOfBillsOfAccountsOfOrder);

            // Remove Bills
            context.Bills.RemoveRange(billsOfAccounts);
            context.Bills.RemoveRange(billsOfAccountsOfOrder);

            // Remove CF
            context.CustomerFeedbacks.RemoveRange(cfOfAccounts);
            context.CustomerFeedbacks.RemoveRange(cfOfAccountsOfOrder);

            // Remove Accounts
            context.Accounts.RemoveRange(accounts);
            context.Accounts.RemoveRange(accountsOfOrder);

            // Remove Orders
            context.Orders.RemoveRange(orders);

            context.Customers.Remove(customer);
            context.SaveChanges();
            TempData["Success"] = "Successfully to delete customer";

            return RedirectToAction("Index");
        }
    }
}