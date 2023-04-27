using eProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace eProject.Areas.Admin.Controllers
{
    public class ContactController : Controller
    {
        NexusEntities context = new NexusEntities();

        // GET: Admin/Contact
        public ActionResult Index()
        {
            List<Contact> contactList = context.Contacts.ToList();
            ViewBag.contactList = contactList;
            return View();
        }

        public ActionResult DetailsContact(int? contactID)
        {
            if (contactID == null) return RedirectToAction("Index");
            Contact contactDetail = context.Contacts.FirstOrDefault(s => s.ContactID == contactID);
            if (contactDetail == null) return RedirectToAction("Index");
            return View(contactDetail);
        }

        public ActionResult Search(string searchValue)
        {
            ViewBag.Contacts = context.Contacts.Where(c => c.ContactName.Contains(searchValue) || searchValue == null).ToList();
            return View();
        }

        
    }
}