using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using eProject.Models.ViewModels.WarehouseEquipment;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class WarehouseEquipmentController : Controller
    {
        NexusEntities context = new NexusEntities();
        // GET: Admin/WarehouseEquipment
        public ActionResult Index()
        {
            List<WarehouseEquipment> warehouseEquipment = context.WarehouseEquipments.ToList();
            ViewBag.WarehouseEquipmentList = warehouseEquipment;
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Store(WarehouseEquipmentViewModel warehouseEquipmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Add");
            }

            if(context.WarehouseEquipments.FirstOrDefault(w => w.WarehouseID == warehouseEquipmentViewModel.WarehouseID && w.EquipmentID == warehouseEquipmentViewModel.EquipmentID) != null)
            {
                TempData["Error"] = "Exists, Please edit";
                return RedirectToAction("Add");
            }

            WarehouseEquipment warehouseEquipment = new WarehouseEquipment();
            warehouseEquipment.WarehouseID = warehouseEquipmentViewModel.WarehouseID;
            warehouseEquipment.EquipmentID = warehouseEquipmentViewModel.EquipmentID;
            warehouseEquipment.Quantity = warehouseEquipmentViewModel.Quantity;

            context.WarehouseEquipments.Add(warehouseEquipment);
            context.SaveChanges();

            TempData["Success"] = "Successfully add new ware equipment";

            return RedirectToAction("Index");

        }

        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            WarehouseEquipment warehouseEquipment = context.WarehouseEquipments.FirstOrDefault(e => e.ID == id);
            if(warehouseEquipment == null)
            {
                return RedirectToAction("Index");
            }

            context.WarehouseEquipments.Remove(warehouseEquipment);
            context.SaveChanges();

            TempData["Success"] = "Success delete warehouse equipment";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            WarehouseEquipment warehouseEquipment = context.WarehouseEquipments.FirstOrDefault(we => we.ID == id);
            if(warehouseEquipment == null)
            {
                return RedirectToAction("Index");
            }

            WarehouseEquipmentViewModel warehouseEquipmentViewModel = new WarehouseEquipmentViewModel();
            warehouseEquipmentViewModel.ID = warehouseEquipment.ID;
            warehouseEquipmentViewModel.WarehouseID = warehouseEquipment.WarehouseID;
            warehouseEquipmentViewModel.EquipmentID = warehouseEquipment.EquipmentID;
            warehouseEquipmentViewModel.Quantity = warehouseEquipment.Quantity;

            return View(warehouseEquipmentViewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Update(WarehouseEquipmentViewModel warehouseEquipment)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }

            WarehouseEquipment warehouseEquipmentUpdate = context.WarehouseEquipments.FirstOrDefault(we => we.ID == warehouseEquipment.ID);
            if (warehouseEquipmentUpdate != null)
            {
                warehouseEquipmentUpdate.WarehouseID = warehouseEquipment.WarehouseID;
                warehouseEquipmentUpdate.EquipmentID = warehouseEquipment.EquipmentID;
                warehouseEquipmentUpdate.Quantity = warehouseEquipment.Quantity;

                context.SaveChanges();
                TempData["Success"] = "Successfully update warehouse equipment";
            }
            return RedirectToAction("Index");
        }
        
    }
}