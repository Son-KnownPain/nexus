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

        // GET: Admin/Service/Delete
        public ActionResult Delete(int? serviceID)
        {
            //if (serviceID == null) return RedirectToAction("Index");
            //Service serviceToDelete = context.Services.FirstOrDefault(s => s.ServiceID == serviceID);
            //if (serviceToDelete == null) return RedirectToAction("Index");

            // Handle remove relationships of Service

            //context.Services.Remove(serviceToDelete);

            TempData["Error"] = "Delete function is not work now";

            return RedirectToAction("Index");
        }

        // POST: Admin/Service/Search
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            ViewBag.services = context.Services.Where(s => s.ServiceName.Contains(keyword)).ToList();
            return View("Index");
        }
    }
}