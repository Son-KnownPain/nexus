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
        private NexusEntities context = new NexusEntities();

        // GET: Admin/WarehouseEquipment
        public ActionResult Index()
        {
            ViewBag.warehouses = context.Warehouses.ToList();
            return View();
        }

        [HttpGet]
        public ActionResult Add(int? warehouseID)
        {
            if (warehouseID == null) return Redirect("/Admin/Warehouse");
            if (context.Warehouses.FirstOrDefault(w => w.WarehouseID == warehouseID) == null) return Redirect("/Admin/Warehouse");

            WarehouseEquipmentViewModel model = new WarehouseEquipmentViewModel();
            model.WarehouseID = (int) warehouseID;
            
            return View(model);
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
                ModelState.AddModelError("EquipmentID", "That warehouse already has this equipment");
                return View("Add");
            }

            WarehouseEquipment warehouseEquipment = new WarehouseEquipment();
            warehouseEquipment.WarehouseID = warehouseEquipmentViewModel.WarehouseID;
            warehouseEquipment.EquipmentID = warehouseEquipmentViewModel.EquipmentID;
            warehouseEquipment.Quantity = warehouseEquipmentViewModel.Quantity;

            context.WarehouseEquipments.Add(warehouseEquipment);
            context.SaveChanges();

            TempData["Success"] = "Successfully add new ware equipment";

            return RedirectToAction("List", new { warehouseID = warehouseEquipment.WarehouseID });

        }

        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return Redirect("/Admin/Warehouse");
            }

            WarehouseEquipment warehouseEquipment = context.WarehouseEquipments.FirstOrDefault(e => e.ID == id);
            if(warehouseEquipment == null)
            {
                return Redirect("/Admin/Warehouse");
            }

            context.WarehouseEquipments.Remove(warehouseEquipment);
            context.SaveChanges();

            TempData["Success"] = "Success delete warehouse equipment";
            return RedirectToAction("List", new { warehouseID = warehouseEquipment.WarehouseID });
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return Redirect("/Admin/Warehouse");
            }

            WarehouseEquipment warehouseEquipment = context.WarehouseEquipments.FirstOrDefault(we => we.ID == id);
            if(warehouseEquipment == null)
            {
                return Redirect("/Admin/Warehouse");
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

            if (context.WarehouseEquipments.FirstOrDefault(w => w.WarehouseID == warehouseEquipment.WarehouseID && w.EquipmentID == warehouseEquipment.EquipmentID) != null)
            {
                TempData["Error"] = "Exists, Please edit";
                return RedirectToAction("Edit");
            }

            WarehouseEquipment warehouseEquipmentUpdate = context.WarehouseEquipments.FirstOrDefault(we => we.ID == warehouseEquipment.ID);
            if (warehouseEquipmentUpdate != null)
            {
                warehouseEquipmentUpdate.Quantity = warehouseEquipment.Quantity;

                context.SaveChanges();
                TempData["Success"] = "Successfully update warehouse equipment";
            }
            return RedirectToAction("List", new { warehouseID = warehouseEquipmentUpdate.WarehouseID });
        }
        
        // GET: Admin/WarehouseEquipment/List
        public ActionResult List(int? warehouseID)
        {
            if (warehouseID == null) return Redirect("/Admin/Warehouse");
            if (context.Warehouses.FirstOrDefault(w => w.WarehouseID == warehouseID) == null) return Redirect("/Admin/Warehouse");
            ViewBag.equipments = context.WarehouseEquipments.Join(context.Equipments, we => we.EquipmentID, e => e.EquipmentID, (we, e) => new WEListViewModel
            {
                ID = we.ID,
                WarehouseID = we.WarehouseID,
                EquipmentID = e.EquipmentID,
                EquipmentName = e.EquipmentName,
                Description = e.Description,
                Image = e.Image,
                Quantity = we.Quantity
            }).Where(x => x.WarehouseID == warehouseID).ToList();

            ViewBag.warehouseID = warehouseID;

            return View();
        }
    }
}