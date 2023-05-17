using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class SupplierController : Controller
    {
        NexusEntities context = new NexusEntities();

        // GET: Admin/Supplier
        public ActionResult Index()
        {
            List<Supplier> supplierList = context.Suppliers.ToList();
            ViewBag.Suppliers = supplierList;
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Store(Supplier addSupplier)
        {
            if (!ModelState.IsValid)
            {
                return View("Add");
            }

            Supplier newSupplier = new Supplier();
            
            newSupplier.CompanyName = addSupplier.CompanyName;
            newSupplier.ContactName = addSupplier.ContactName;
            newSupplier.Address = addSupplier.Address;
            newSupplier.Phone = addSupplier.Phone;
            newSupplier.Fax = addSupplier.Fax;
            newSupplier.ContactUrl = addSupplier.ContactUrl;

            context.Suppliers.Add(newSupplier);
            context.SaveChanges();

            TempData["Success"] = "Success add new supplier";
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            Supplier updateSupplier = context.Suppliers.FirstOrDefault(s => s.SupplierID == id);
            if(updateSupplier == null)
            {
                return RedirectToAction("Index");
            }
            return View(updateSupplier);
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }

            Supplier updateSupllier = context.Suppliers.FirstOrDefault(s => s.SupplierID == supplier.SupplierID);
            if(updateSupllier != null)
            {
                updateSupllier.CompanyName = supplier.CompanyName;
                updateSupllier.ContactName = supplier.ContactName;
                updateSupllier.Address = supplier.Address;
                updateSupllier.Phone = supplier.Phone;
                updateSupllier.Fax = supplier.Fax;
                updateSupllier.ContactUrl = supplier.ContactUrl;

                context.SaveChanges();
                TempData["Success"] = "Success update Supplier";

            }
            return Redirect("Index");
        }

        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            Supplier deleteSupplier = context.Suppliers.FirstOrDefault(s => s.SupplierID == id);
            if(deleteSupplier == null) return RedirectToAction("Index");

            var equipments = context.Equipments.Where(e => e.SupplierID == deleteSupplier.SupplierID);

            context.WarehouseEquipments.RemoveRange(context.WarehouseEquipments.Where(we => equipments.Select(e => e.EquipmentID).ToList().Contains(we.EquipmentID)));
            context.Equipments.RemoveRange(equipments);

            context.Suppliers.Remove(deleteSupplier);

            context.SaveChanges();

            TempData["Success"] = "Successfully to delete supplier";

            return RedirectToAction("Index");
        }

        

        public ActionResult Search(string keyword)
        {
            if(!string.IsNullOrEmpty(keyword))
            {
                ViewBag.Suppliers = context.Suppliers.Where(s => s.CompanyName.Contains(keyword) || s.ContactName.Contains(keyword)).ToList();
            }
            else
            {
                TempData["Error"] = "No keyword entered, please enter your keyword";
                return RedirectToAction("Index");
            }

            return View("Index");
        }
    }
}