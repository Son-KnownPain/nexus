using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using eProject.Auth;
using eProject.Filters;
using eProject.Models.ViewModels.WarehouseEquipment;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]

    public class WarehouseController : Controller
    { 
        NexusEntities context = new NexusEntities();
         
        // GET: Admin/Warehouse
        public ActionResult Index()
        {
            List<Warehouse> warehouseItem = context.Warehouses.ToList();
            
            ViewBag.warehouseList = warehouseItem;
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Store(Warehouse whModel)
        {
            if(!ModelState.IsValid)
            {
                return View("Add");
            }

            Warehouse newWarehouse = new Warehouse();
            newWarehouse.Name = whModel.Name;
            newWarehouse.Address = whModel.Address;
            newWarehouse.ContactNumber = whModel.ContactNumber;

            context.Warehouses.Add(newWarehouse);
            context.SaveChanges();
            TempData["Success"] = "Successfully add new warehouse";

            return Redirect("Index");
        }

        [AdministratorAuthorization]
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            Warehouse warehouseDelete = context.Warehouses.FirstOrDefault(w => w.WarehouseID == id);
            if(warehouseDelete == null) return RedirectToAction("Index");

            context.WarehouseEquipments.RemoveRange(context.WarehouseEquipments.Where(a => a.WarehouseID == warehouseDelete.WarehouseID));

            context.Warehouses.Remove(warehouseDelete);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete warehouse";
            return RedirectToAction("Index");
        }

        [AdministratorAuthorization]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Warehouse warehouseEdit = context.Warehouses.FirstOrDefault(w => w.WarehouseID == id);
            if (warehouseEdit == null) { return RedirectToAction("Index"); }

            return View(warehouseEdit);
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        [AdministratorAuthorization]
        public ActionResult Update(Warehouse updateWarehouse)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }

            Warehouse newWarehouse = context.Warehouses.FirstOrDefault(w => w.WarehouseID == updateWarehouse.WarehouseID); 
            if (newWarehouse != null) { 

                newWarehouse.Name = updateWarehouse.Name;
                newWarehouse.Address = updateWarehouse.Address;
                newWarehouse.ContactNumber = updateWarehouse.ContactNumber;

                context.SaveChanges();
                TempData["Success"] = "Successfully update warehouse";

            }
            return Redirect("Index");
        }

        public ActionResult Search(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.warehouseList = context.Warehouses.Where(w => w.Name.Contains(keyword) || w.Address.Contains(keyword)).ToList();
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