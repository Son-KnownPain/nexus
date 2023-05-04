using eProject.Filters;
using eProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Auth;
using eProject.Models.ViewModels.Equipment;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]

    public class EquipmentController : Controller
    {
        NexusEntities context = new NexusEntities();
        // GET: Admin/Equipment
        public ActionResult Index()
        {
            List<Equipment> equipmentList = context.Equipments.ToList();
            ViewBag.EquipmentList = equipmentList;
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {

            EquipmentViewModel equipmentViewModel = new EquipmentViewModel();

            return View(equipmentViewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Store(EquipmentViewModel equipment, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Add");
            }

            Equipment newEquipment = new Equipment();

            newEquipment.EquipmentName = equipment.EquipmentName;
            newEquipment.SupplierID = equipment.SupplierID;
            newEquipment.Description = equipment.Description;
            
            if(imageFile != null && imageFile.ContentLength > 0)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                String uploadFolderPath = Server.MapPath("~/Uploads/EquipmentImage");

                String extName = Path.GetExtension(imageFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();

                String newFlieName = prefix + "equipment-image" + randomNumber + extName;
                imageFile.SaveAs(uploadFolderPath + "/" + newFlieName);

                newEquipment.Image = newFlieName;
            }
            else
            {
                return RedirectToAction("Add");
            }

            context.Equipments.Add(newEquipment);
            context.SaveChanges();

            TempData["Success"] = "Successfully add new equipment";
            return Redirect("Index");
        }

        [AdministratorAuthorization]
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            Equipment equipmentDelete = context.Equipments.FirstOrDefault(e => e.EquipmentID == id);
            if(equipmentDelete == null)
            {
                return RedirectToAction("Index");
            }

            //Delete relationship
            context.WarehouseEquipments.RemoveRange(context.WarehouseEquipments.Where(e => e.EquipmentID == id));

            context.Equipments.Remove(equipmentDelete);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete equipment";
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

            Equipment equipment = context.Equipments.FirstOrDefault(e => e.EquipmentID == id);
            if(equipment == null)
            {
                return RedirectToAction("Index");
            }

            EquipmentViewModel equipmentViewModel = new EquipmentViewModel();
            equipmentViewModel.EquipmentID = equipment.EquipmentID;
            equipmentViewModel.SupplierID = equipment.SupplierID;
            equipmentViewModel.EquipmentName = equipment.EquipmentName;
            equipmentViewModel.Description = equipment.Description;
            equipmentViewModel.Image = equipment.Image;

            return View(equipmentViewModel);
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(EquipmentViewModel updateEquipment, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }

            Equipment equipment = context.Equipments.FirstOrDefault(e => e.EquipmentID == updateEquipment.EquipmentID);
            if (equipment != null)
            {
                equipment.EquipmentName = updateEquipment.EquipmentName;
                equipment.SupplierID = updateEquipment.SupplierID;
                equipment.Description = updateEquipment.Description;

                if(imageFile != null && imageFile.ContentLength > 0)
                {
                    String uploadFolderPath = Server.MapPath("~/Uploads/EquipmentImage");

                    //delete ảnh cũ
                    String oldFileName = equipment.Image;
                    if (System.IO.File.Exists(uploadFolderPath + "/" + oldFileName))
                    {
                        System.IO.File.Delete(uploadFolderPath + "/" + oldFileName);
                    }
                    String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                    String extName = Path.GetFileName(imageFile.FileName);

                    Random random = new Random();
                    int randomNumber = random.Next();

                    String newFileName = prefix + "equipment-image" + randomNumber + extName;
                    imageFile.SaveAs(uploadFolderPath + "/" + newFileName);

                    equipment.Image = newFileName;
                }

                context.SaveChanges();
                TempData["Success"] = "Successfully update equipment";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Search(string keyword)
        {
            ViewBag.EquipmentList = context.Equipments.Where(e => e.EquipmentName.Contains(keyword)).ToList();
            return View("Index");
        }
    }
}