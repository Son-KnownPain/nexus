using eProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using eProject.Filters;
using System.Web.Razor.Tokenizer.Symbols;
using eProject.Models.ViewModels.Employee;
using System.Web.Helpers;
using eProject.Auth;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class RetailShowRoomController : Controller
    {
        NexusEntities context = new NexusEntities();

        // GET: Admin/RetailShowRoom
        public ActionResult Index()
        {
            List<RetailShowRoom> retailShowRoomList = context.RetailShowRooms.ToList();
            ViewBag.RetailShowRooms = retailShowRoomList;
            return View();
        }

        public ActionResult AddRetailStore()
        {
            return View();
        }


        // Post: /RetailStore/Store
        [HttpPost]
        public ActionResult Store(RetailShowRoom retailShowRoom, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) return View("AddRetailStore");


            RetailShowRoom newRetailStore = new RetailShowRoom();
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                String uploadFolderPath = Server.MapPath("~/Uploads/RetailShowRoomImage");

                String extName = Path.GetExtension(imageFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "retailShowRoom-image" + randomNumber + extName;

                imageFile.SaveAs(uploadFolderPath + "/" + newFileName);

                newRetailStore.Image = newFileName;
            }
            else
            {
                TempData["Error"] = "You must upload retail show room image";
                return RedirectToAction("AddRetailStore");
            }

            newRetailStore.Name = retailShowRoom.Name;
            newRetailStore.Phone = retailShowRoom.Phone;
            newRetailStore.Address = retailShowRoom.Address;
            newRetailStore.EmployeeQuantity = 0;

            context.RetailShowRooms.Add(newRetailStore);
            context.SaveChanges();

            TempData["Success"] = "Successfully add new retail show room";
            return Redirect("Index");

        }

        public ActionResult Delete(int? retailShowRoomID)
        {
            if (retailShowRoomID == null)
            {
                return RedirectToAction("Index");
            }
            RetailShowRoom retailShowDelete = context.RetailShowRooms.FirstOrDefault(r => r.RetailShowRoomID == retailShowRoomID);
            if (retailShowDelete == null) 
            { 
                return RedirectToAction("Index"); 
            }

            List<Employee> employees = context.Employees.Where(e => e.RetailShowRoomID == retailShowRoomID).ToList();

            employees.ForEach(e =>
            {
                e.RetailShowRoomID = null;
            });

            context.RetailShowRooms.Remove(retailShowDelete);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete retail show room";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditRetailStore(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            RetailShowRoom edit = context.RetailShowRooms.FirstOrDefault(r => r.RetailShowRoomID == id);
            if (edit == null) return RedirectToAction("Index");

            return View(edit);
        }

        [HttpPut, ValidateInput(false)]
        public ActionResult Update(RetailShowRoom updateRetailShowRoom, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) return View("EditRetailStore");

            RetailShowRoom newRetailStore = context.RetailShowRooms.FirstOrDefault(r => r.RetailShowRoomID == updateRetailShowRoom.RetailShowRoomID);
            if (newRetailStore != null)
            {
                newRetailStore.Name = updateRetailShowRoom.Name;
                newRetailStore.Phone = updateRetailShowRoom.Phone;
                newRetailStore.Address = updateRetailShowRoom.Address;
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    String uploadFolderPath = Server.MapPath("~/Uploads/RetailShowRoomImage");

                    //delete ảnh cũ
                    String oldFileName = newRetailStore.Image;
                    if (System.IO.File.Exists(uploadFolderPath + "/" + oldFileName))
                    {
                        System.IO.File.Delete(uploadFolderPath + "/" + oldFileName);
                    }

                    String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                    String extName = Path.GetExtension(imageFile.FileName);
                    
                    Random random = new Random();
                    int randomNumber = random.Next();
                    String newFileName = prefix + "retailShowRoom-image" + randomNumber + extName;

                    imageFile.SaveAs(uploadFolderPath + "/" + newFileName);

                    newRetailStore.Image = newFileName;
                }
                context.SaveChanges();
                TempData["Success"] = "Successfully update retail show room";
            }

            return RedirectToAction("Index");
        }

        public ActionResult Search(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.RetailShowRooms = context.RetailShowRooms.Where(r => r.Name.Contains(keyword) || r.Address.Contains(keyword)).ToList();
            }
            else
            {
                TempData["Error"] = "No keyword entered, please enter your keyword";
                return RedirectToAction("Index");
            }
            
            return View("Index");
        }

        public ActionResult Detail(int? retailShowRoomID)
        {
            if (retailShowRoomID == null)
            {
                return RedirectToAction("Index");
            }
            RetailShowRoom retailShowRoom = context.RetailShowRooms.FirstOrDefault(r => r.RetailShowRoomID == retailShowRoomID);
            if (retailShowRoom == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.rsrID = retailShowRoomID;
            ViewBag.Employees = context.Employees.Where(r => r.RetailShowRoomID == retailShowRoomID).ToList();
            return View("Detail");
        }

        public ActionResult AddRetailStoreEmployee(int? rsrID)
        {
            if(rsrID == null)
            {
                return RedirectToAction("Index");
            }
            RetailShowRoom retailShowRoom = context.RetailShowRooms.FirstOrDefault(r => r.RetailShowRoomID == rsrID);
            if(retailShowRoom == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.rsrID = rsrID;
            ViewBag.employees = context.Employees.Where(e => e.RetailShowRoomID == null && e.Role == AuthManager.EmployeeRoles.RetailStoreEmployeeRole).ToList();
            return View();
        }

        public ActionResult StoreEmployee(int? employeeID, int? rsrID)
        {
            if(employeeID == null || rsrID == null) return RedirectToAction("Index");

            Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeID == employeeID);
            RetailShowRoom retailShowRoom = context.RetailShowRooms.FirstOrDefault(r => r.RetailShowRoomID == rsrID);

            if(employee == null || retailShowRoom == null) return RedirectToAction("Index");
            
            employee.RetailShowRoomID = rsrID;
            retailShowRoom.EmployeeQuantity += 1;

            context.SaveChanges();

            TempData["Success"] = "Successfully add new retail store eployee";
            return RedirectToAction("Detail", new { retailShowRoomID = rsrID });
        }

        public ActionResult Remove(int? employeeID, int? rsrID)
        {
            if (employeeID == null || rsrID == null) return RedirectToAction("Index");

            Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeID == employeeID);
            RetailShowRoom retailShowRoom = context.RetailShowRooms.FirstOrDefault(r => r.RetailShowRoomID == rsrID);

            if (employee == null || retailShowRoom == null || employee.RetailShowRoomID != rsrID) return RedirectToAction("Index");

            employee.RetailShowRoomID = null;
            retailShowRoom.EmployeeQuantity -= 1;

            context.SaveChanges();

            TempData["Success"] = "Successfully add new retail store eployee";
            return RedirectToAction("Detail", new { retailShowRoomID = rsrID });
        }

        public ActionResult SearchRSE(string keyword)
        {

            ViewBag.employees = context.Employees.Where(r => r.Fullname.Contains(keyword) && r.Role == AuthManager.EmployeeRoles.RetailStoreEmployeeRole).ToList();
            return View("Detail");

        }

        public ActionResult SearchAdd(string keyword)
        {
            ViewBag.employees = context.Employees.Where(r => r.Fullname.Contains(keyword) && r.Role == AuthManager.EmployeeRoles.RetailStoreEmployeeRole).ToList();

            return View("AddRetailStoreEmployee");
        }
    }
}