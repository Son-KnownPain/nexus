using eProject.Filters;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class ServiceController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/Service
        public ActionResult Index()
        {
            List<Service> serviceList = context.Services.ToList();
            ViewBag.services = serviceList;
            return View();
        }

        // GET: Admin/Service/Add
        public ActionResult Add()
        {
            return View();
        }

        // POST: Admin/Service/Store
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Store(Service serviceData, HttpPostedFileBase ThumbnailFile)
        {
            if (!ModelState.IsValid) return View("Add");


            if (ThumbnailFile != null && ThumbnailFile.ContentLength > 0)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                String uploadFolderPath = Server.MapPath("~/Uploads/ServiceThumbnails");

                String extName = Path.GetExtension(ThumbnailFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "service-thumbnail" + randomNumber + extName;

                ThumbnailFile.SaveAs(uploadFolderPath + "/" + newFileName);

                Service service = new Service();
                service.ServiceName = serviceData.ServiceName;
                service.Deposit = serviceData.Deposit;
                service.Summary = serviceData.Summary;
                service.Description = serviceData.Description;
                service.Thumbnail = newFileName;

                context.Services.Add(service);

                try
                {
                    context.SaveChanges();
                }
                catch(Exception e)
                {
                    throw e;
                }

                TempData["Success"] = "Successfully add new service";

                return RedirectToAction("Index");

            }
            else
            {
                TempData["Error"] = "Please upload service thumbnail";
                return RedirectToAction("Add");
            }
        }

        // GET: Admin/Service/Edit
        public ActionResult Edit(int? serviceID)
        {
            if (serviceID == null) return RedirectToAction("Index");
            Service serviceToEdit = context.Services.FirstOrDefault(s => s.ServiceID == serviceID);
            if (serviceToEdit == null) return RedirectToAction("Index");
            return View(serviceToEdit);
        }

        // PUT: Admin/Service/Update
        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(Service serviceData, HttpPostedFileBase ThumbnailFile)
        {
            if (!ModelState.IsValid) return View("Edit");

            Service service = context.Services.FirstOrDefault(s => s.ServiceID == serviceData.ServiceID);
            if (service == null)
            {
                TempData["Error"] = "Service ID not found";
                return RedirectToAction("Index");
            }
            service.ServiceName = serviceData.ServiceName;
            service.Deposit = serviceData.Deposit;
            service.Summary = serviceData.Summary;
            service.Description = serviceData.Description;
            

            if (ThumbnailFile != null && ThumbnailFile.ContentLength > 0)
            {
                String uploadFolderPath = Server.MapPath("~/Uploads/ServiceThumbnails");

                // Delete old file
                String oldFileName = service.Thumbnail;
                if (System.IO.File.Exists(uploadFolderPath + "/" + oldFileName))
                {
                    System.IO.File.Delete(uploadFolderPath + "/" + oldFileName);
                }

                String extName = Path.GetExtension(ThumbnailFile.FileName);
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "service-thumbnail" + randomNumber + extName;

                ThumbnailFile.SaveAs(uploadFolderPath + "/" + newFileName);

                service.Thumbnail = newFileName;
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            TempData["Success"] = "Successfully update service";

            return RedirectToAction("Index");
        }

        
        // POST: Admin/Service/Search
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.services = context.Services.Where(s => s.ServiceName.Contains(keyword)).ToList();
            }
            else
            {
                TempData["Error"] = "No keyword entered, please enter your keyword";
                return RedirectToAction("Index");
            }
            
            return View("Index");
        }

        // GET: Admin/Service/Delete
        public ActionResult Delete(int? serviceID)
        {
            if (serviceID == null)
            {
                return RedirectToAction("Index");
            }

            Service service = context.Services.FirstOrDefault(s => s.ServiceID == serviceID);
            if (service == null)
            {
                return RedirectToAction("Index");
            }

            // Order -> Service
            var ordersOfService = context.Orders.Where(x => x.ServiceID == service.ServiceID);

            // Accounts -> Service
            var accountsOfService = context.Accounts.Where(x => x.ServiceID == service.ServiceID);

            // PL -> Service
            var pls = context.PaymentPlans.Where(x => x.ServiceID == service.ServiceID);

            // PLD -> PL
            var plds = context.PaymentPlanDetails.Where(x => pls.Select(a => a.PaymentPlanID).ToList().Contains(x.PaymentPlanID));

            // Call charge -> PLD
            var callCharges = context.CallCharges.Where(x => plds.Select(s => s.PaymentPlanDetailID).ToList().Contains(x.PaymentPlanDetailID));

            // ------------------------------------------------------------

            // Order -> PLD
            var ordersOfPLDs = context.Orders.Where(x => plds.Select(s => s.PaymentPlanDetailID).ToList().Contains(x.PaymentPlanDetailID));
            // Account -> Order
            var accountsOfOrder = context.Accounts.Where(x => ordersOfPLDs.Select(o => o.OrderID).ToList().Contains(x.OrderID));
            // Bill -> Account
            var billsOfAccountsOfOrder = context.Bills.Where(x => accountsOfOrder.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // CustomerFeedback -> Account
            var cfOfAccountsOfOrder = context.CustomerFeedbacks.Where(x => accountsOfOrder.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // Charge -> Bill
            var chargesOfBillsOfAccountsOfOrder = context.Charges.Where(x => billsOfAccountsOfOrder.Select(a => a.BillID).ToList().Contains(x.BillID));

            // ------------------------------------------------------------

            // Account -> PLD
            var accountsOfPLDs = context.Accounts.Where(x => plds.Select(s => s.PaymentPlanDetailID).ToList().Contains(x.PaymentPlanDetailID));
            // Bill -> Account
            var billsOfAccounts = context.Bills.Where(x => accountsOfPLDs.Select(a => a.AccountID).ToList().Contains(x.AccountID));
            // CustomerFeedback -> Account
            var cfOfAccounts = context.CustomerFeedbacks.Where(x => accountsOfPLDs.Select(a => a.AccountID).ToList().Contains(x.AccountID));
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
            context.Accounts.RemoveRange(accountsOfPLDs);
            context.Accounts.RemoveRange(accountsOfOrder);
            context.Accounts.RemoveRange(accountsOfService);

            // Remove Orderes
            context.Orders.RemoveRange(ordersOfPLDs);
            context.Orders.RemoveRange(ordersOfService);

            // Remove Call Charges
            context.CallCharges.RemoveRange(callCharges);

            // Remove Payment Plan Detail
            context.PaymentPlanDetails.RemoveRange(plds);

            // Remove Payment Plan
            context.PaymentPlans.RemoveRange(pls);

            context.Services.Remove(service);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete service";
            return RedirectToAction("Index");
        }
    }
}