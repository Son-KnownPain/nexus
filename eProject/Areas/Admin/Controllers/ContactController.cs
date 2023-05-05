using eProject.Filters;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using eProject.Filters;

namespace eProject.Areas.Admin.Controllers
{
    [EmployeeAuthorization]
    public class ContactController : Controller
    {
        NexusEntities context = new NexusEntities();

        // GET: Admin/Contact
        public ActionResult Index()
        {
            List<Contact> contactList = context.Contacts.OrderByDescending(c => c.ContactID).ToList();
            ViewBag.contactList = contactList;
            return View();
        }

        public ActionResult DetailsContact(int? contactID)
        {
            if (contactID == null) return RedirectToAction("Index");
            Contact contactDetail = context.Contacts.FirstOrDefault(s => s.ContactID == contactID);
            if (contactDetail == null) return RedirectToAction("Index");
            contactDetail.Status = "Seen";
            context.SaveChanges();
            return View(contactDetail);
        }

  
        public ActionResult Delete(int? deleteContact)
        {
            if(deleteContact == null)
            {
                return RedirectToAction("Index");
            }

            Contact contact = context.Contacts.FirstOrDefault(c => c.ContactID == deleteContact);
            if (contact == null) return RedirectToAction("Index");

            context.Contacts.Remove(contact);
            context.SaveChanges();
            TempData["Success"] = "Success delete contact";

            return RedirectToAction("Index");
        }

        public ActionResult Search(string searchValue)
        {
            ViewBag.contactList = context.Contacts.Where(c => c.ContactName.Contains(searchValue) || c.Content.Contains(searchValue)).ToList();
            return View("Index");
        }
    }
}