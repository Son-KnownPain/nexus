using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Models;

namespace eProject.Controllers
{
    public class ContactController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        // POST: Contact/Store
        [HttpPost]
        public ActionResult Store(Contact contactData)
        {
            if (!ModelState.IsValid) return View("Index");

            Contact contact = new Contact();
            contact.ContactName = contactData.ContactName;
            contact.Phone = contactData.Phone;
            contact.Email = contactData.Email;
            contact.Content = contactData.Content;
            contact.Status = "Sent";
            contact.ContactedAt = DateTime.Now;

            context.Contacts.Add(contact);

            try
            {
                context.SaveChanges();
            }
            catch(Exception e)
            {
                throw e;
            }

            TempData["Success"] = "Successfully send your contact information to us";
            return RedirectToAction("Index");
        }
    }
}