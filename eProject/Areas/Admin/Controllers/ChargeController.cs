using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using eProject.Models;
using Microsoft.Ajax.Utilities;

namespace eProject.Areas.Admin.Controllers
{
    [EmployeeAuthorization]
    public class ChargeController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/Charge/Add
        public ActionResult Add(int? billID)
        {
            if (billID == null) return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/Admin/Bill");

            Bill bill = context.Bills.FirstOrDefault(a => a.BillID == billID);

            if (bill == null) return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/Admin/Bill");

            Charge model = new Charge();

            model.BillID = bill.BillID;

            return View(model);
        }

        // POST: Admin/Charge/Store
        [HttpPost]
        public ActionResult Store(Charge data)
        {
            if (!ModelState.IsValid) return View("Add");

            Bill bill = context.Bills.FirstOrDefault(a => a.BillID == data.BillID);

            if (bill == null) return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/Admin/Bill");

            Charge charge = new Charge();

            charge.BillID = data.BillID;
            charge.ChargeName = data.ChargeName;
            charge.Value = data.Value;
            charge.Description = data.Description;
            charge.CreatedAt = DateTime.Now;

            bill.AmountPaid += (double)charge.Value / 100;

            context.Charges.Add(charge);

            context.SaveChanges();

            TempData["Success"] = "Successfully add a new charge";

            return Redirect("/Admin/Bill/Edit?billID=" + charge.BillID);
        }

        // GET: Admin/Charge/Edit
        public ActionResult Edit(int? chargeID)
        {
            if (chargeID == null) return Redirect("/Admin/Bill");

            Charge charge = context.Charges.FirstOrDefault(c => c.ChargeID == chargeID);

            if (charge == null) return Redirect("/Admin/Bill");

            return View(charge);
        }

        // PUT: Admin/Charge/Update
        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult Update(Charge data)
        {
            if (!ModelState.IsValid) return View("Add");

            Bill bill = context.Bills.FirstOrDefault(a => a.BillID == data.BillID);

            Charge charge = context.Charges.FirstOrDefault(c => c.ChargeID == data.ChargeID);

            if (charge == null || charge.BillID != bill.BillID) return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "/Admin/Bill");

            bill.AmountPaid -= (double)charge.Value / 100;

            charge.ChargeName = data.ChargeName;
            charge.Value = data.Value;
            charge.Description = data.Description;
            charge.CreatedAt = DateTime.Now;
            
            bill.AmountPaid += (double)charge.Value / 100;

            context.SaveChanges();

            TempData["Success"] = "Successfully udpate charge";

            return Redirect("/Admin/Bill/Edit?billID=" + charge.BillID);
        }

        // GET: Admin/Charge/Delete
        public ActionResult Delete(int? chargeID)
        {
            if (chargeID == null) return Redirect("/Admin/Bill");

            Charge charge = context.Charges.FirstOrDefault(c => c.ChargeID == chargeID);

            if (charge == null) return Redirect("/Admin/Bill");

            Bill bill = context.Bills.FirstOrDefault(a => a.BillID == charge.BillID);

            bill.AmountPaid -= (double)charge.Value / 100;

            context.Charges.Remove(charge);

            context.SaveChanges();

            TempData["Success"] = "Successfully delete charge";

            return Redirect("/Admin/Bill/Edit?billID=" + charge.BillID);
        }
    }
}