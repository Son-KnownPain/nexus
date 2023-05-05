using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using eProject.Auth;
using eProject.Models;
using eProject.Filters;
using Microsoft.Ajax.Utilities;

namespace eProject.Areas.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        private NexusEntities context = new NexusEntities();
         
        // GET: Admin/Employee
        [AdministratorAuthorization]
        public ActionResult Index()
        {
            ViewBag.employees = context.Employees.ToList();
            return View();
        }

        // GET: Admin/Employee/Search
        [AdministratorAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string keyword)
        {
            ViewBag.employees = context.Employees
                .Where(e =>
                    e.Phone.Contains(keyword) ||
                    e.Fullname.Contains(keyword) ||
                    e.Email.Contains(keyword) ||
                    e.Address.Contains(keyword)
                )
                .ToList();
            return View("Index");
        }

        // GET: Admin/Employee/SignIn
        public ActionResult SignIn()
        {
            return View();
        }

        // POST: Admin/Employee/Check
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Check(SimpleProxyEmployee data)
        {
            if (!ModelState.IsValid) return View("SignIn");

            Employee employee = context.Employees.FirstOrDefault(e => e.Phone == data.Phone);
            if (employee != null && Crypto.VerifyHashedPassword(employee.Password, data.Password))
            {
                AuthManager.Chef.PreservationEmployeeCookies(employee);
                AuthManager.CurrentEmployee.Update(employee);
                TempData["Success"] = "Successfully sign in to administration app";
                return Redirect("/Admin");
            }

            TempData["Error"] = "Phone or password incorrect";
            return RedirectToAction("SignIn");
        }

        // GET: Admin/Employee/SignOut
        public ActionResult SignOut()
        {
            AuthManager.Chef.SpoiledEmployee();
            return Redirect("/Admin");
        }

        // GET: Admin/Employee/Add
        [AdministratorAuthorization]
        public ActionResult Add()
        {
            return View();
        }

        // POST: Admin/Employee/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdministratorAuthorization]
        public ActionResult Store(EmployeeInteraction data, HttpPostedFileBase AvatarFile)
        {
            if (!ModelState.IsValid) return View("Add");

            bool isPhoneExisting = context.Employees.FirstOrDefault(e => e.Phone == data.Phone) != null;
            if (isPhoneExisting)
            {
                ModelState.AddModelError("Phone", "Phone existing, try other phone");
                return View("Add");
            }

            Employee employee = new Employee();

            employee.RetailShowRoomID = null;
            employee.Fullname = data.Fullname;
            employee.StillWorking = true;
            switch (data.Role)
            {
                case 1:
                    employee.Role = 1;
                    employee.RoleName = "Retail Store Employee";
                    employee.Department = "Retail Store Department";
                    break;
                case 2:
                    employee.Role = 2;
                    employee.RoleName = "Technical Employee";
                    employee.Department = "Technical Department";
                    break;
                case 3:
                    employee.Role = 3;
                    employee.RoleName = "Account Employee";
                    employee.Department = "Account Department";
                    break;
                case 10:
                    employee.Role = 10;
                    employee.RoleName = "Admin Employee";
                    employee.Department = "Administration Department";
                    break;
                default:
                    employee.Role = 1;
                    employee.RoleName = "Retail Store Employee";
                    employee.Department = "Retail Store Department";
                    break;
            }
            employee.Phone = data.Phone;
            employee.Email = data.Email;
            employee.Password = Crypto.HashPassword(data.Password);
            employee.Address = data.Address;
            employee.CreatedAt = DateTime.Now;
            employee.UpdatedAt = DateTime.Now;

            if (AvatarFile != null && AvatarFile.ContentLength > 0)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                String uploadFolderPath = Server.MapPath("~/Uploads/EmployeeAvatars");

                String extName = Path.GetExtension(AvatarFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "employee-avatar" + randomNumber + extName;

                AvatarFile.SaveAs(uploadFolderPath + "/" + newFileName);

                employee.Avatar = newFileName;
            }
            else
            {
                employee.Avatar = "default-employee-avatar.png";
            }

            // Add and save changes

            context.Employees.Add(employee);

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            TempData["Success"] = "Successfully add new employee";

            return RedirectToAction("Index");
        }

        // GET: Admin/Employee/Edit
        [AdministratorAuthorization]
        public ActionResult Edit(int? employeeID)
        {
            if (employeeID == null) return RedirectToAction("Index");
            Employee employeeData = context.Employees.FirstOrDefault(e => e.EmployeeID == employeeID);
            if (employeeData == null) return RedirectToAction("Index");

            EmployeeViewEditModel model = new EmployeeViewEditModel();

            model.EmployeeID = employeeData.EmployeeID;
            model.StillWorking = employeeData.StillWorking;
            model.Fullname = employeeData.Fullname;
            model.Role = employeeData.Role;
            model.Phone = employeeData.Phone;
            model.Email = employeeData.Email;
            model.Address = employeeData.Address;
            model.Avatar = employeeData.Avatar;

            return View(model);
        }

        // PUT: Admin/Employee/Update
        [AdministratorAuthorization]
        public ActionResult Update(EmployeeViewEditModel data, HttpPostedFileBase AvatarFile)
        {
            if (!ModelState.IsValid) return View("Add");

            bool isPhoneExisting = context.Employees.FirstOrDefault(e => e.Phone == data.Phone && e.EmployeeID != data.EmployeeID) != null;
            if (isPhoneExisting)
            {
                ModelState.AddModelError("Phone", "Phone existing, try other phone");
                return View("Add");
            }

            Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeID == data.EmployeeID);

            if (employee == null) return RedirectToAction("Index");

            employee.Fullname = data.Fullname;
            employee.StillWorking = data.StillWorking;
            switch (data.Role)
            {
                case 1:
                    employee.Role = 1;
                    employee.RoleName = "Retail Store Employee";
                    employee.Department = "Retail Store Department";
                    break;
                case 2:
                    employee.Role = 2;
                    employee.RoleName = "Technical Employee";
                    employee.Department = "Technical Department";
                    break;
                case 3:
                    employee.Role = 3;
                    employee.RoleName = "Account Employee";
                    employee.Department = "Account Department";
                    break;
                case 10:
                    employee.Role = 10;
                    employee.RoleName = "Admin Employee";
                    employee.Department = "Administration Department";
                    break;
                default:
                    employee.Role = 1;
                    employee.RoleName = "Retail Store Employee";
                    employee.Department = "Retail Store Department";
                    break;
            }
            employee.Phone = data.Phone;
            employee.Email = data.Email;
            employee.Address = data.Address;
            employee.UpdatedAt = DateTime.Now;

            if (AvatarFile != null && AvatarFile.ContentLength > 0)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                String uploadFolderPath = Server.MapPath("~/Uploads/EmployeeAvatars");

                String extName = Path.GetExtension(AvatarFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "employee-avatar" + randomNumber + extName;

                AvatarFile.SaveAs(uploadFolderPath + "/" + newFileName);

                // Delete old file
                if (System.IO.File.Exists(uploadFolderPath + "/" + employee.Avatar) && !employee.Avatar.Equals("default-employee-avatar.png"))
                {
                    System.IO.File.Delete(uploadFolderPath + "/" + employee.Avatar);
                }

                employee.Avatar = newFileName;
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            TempData["Success"] = "Successfully update employee information";

            return RedirectToAction("Index");
        }

        // GET: Admin/Employee/Delete
        [AdministratorAuthorization]
        public ActionResult Delete(int? employeeID)
        {
            if (employeeID == null) return RedirectToAction("Index");
            Employee employeeData = context.Employees.FirstOrDefault(e => e.EmployeeID == employeeID);
            if (employeeData == null) return RedirectToAction("Index");

            // Delete avatar
            String uploadFolderPath = Server.MapPath("~/Uploads/EmployeeAvatars");
            if (System.IO.File.Exists(uploadFolderPath + "/" + employeeData.Avatar) && !employeeData.Avatar.Equals("default-employee-avatar.png"))
            {
                System.IO.File.Delete(uploadFolderPath + "/" + employeeData.Avatar);
            }

            context.Employees.Remove(employeeData);

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            TempData["Success"] = "Successfully delete employee";

            return RedirectToAction("Index");
        }

        // GET: Admin/Employee/ChangePassword
        [AdministratorAuthorization]
        public ActionResult ChangePassword(int? employeeID)
        {
            if (employeeID == null) return RedirectToAction("Index");
            Employee employeeData = context.Employees.FirstOrDefault(e => e.EmployeeID == employeeID);
            if (employeeData == null) return RedirectToAction("Index");

            ViewBag.EmployeeName = employeeData.Fullname;

            PasswordEditViewModel model = new PasswordEditViewModel();

            model.EmployeeID = employeeData.EmployeeID;

            return View(model);
        }

        // PUT: Admin/Employee/UpdatePassword
        [AdministratorAuthorization]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(PasswordEditViewModel data)
        {
            if (!ModelState.IsValid) return View("ChangePassword");

            try
            {
                Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeID == data.EmployeeID);
                if (employee == null) return RedirectToAction("Index");

                employee.Password = Crypto.HashPassword(data.NewPassword);
                employee.UpdatedAt = DateTime.Now;

                context.SaveChanges();

                TempData["Success"] = "Successfully update employee password";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: Admin/Employee/MyProfile
        [EmployeeAuthorization]
        public ActionResult MyProfile()
        {
            int MyProfileID = AuthManager.CurrentEmployee.GetEmployee.EmployeeID;
            Employee model = context.Employees.FirstOrDefault(e => e.EmployeeID == MyProfileID);
            if (model == null) return Redirect("/Admin");
            return View(model);
        }

        // PUT: Admin/Employee/ChangeMyAvatar
        [EmployeeAuthorization]
        [ValidateAntiForgeryToken, HttpPut]
        public ActionResult ChangeMyAvatar(HttpPostedFileBase AvatarFile)
        {
            int id = AuthManager.CurrentEmployee.GetEmployee.EmployeeID;
            Employee employee = context.Employees.FirstOrDefault(e => e.EmployeeID == id);

            if (AvatarFile != null && AvatarFile.ContentLength > 0)
            {
                String prefix = DateTime.Now.ToString("ddMMyyyyHHmmssffff_");
                String uploadFolderPath = Server.MapPath("~/Uploads/EmployeeAvatars");

                String extName = Path.GetExtension(AvatarFile.FileName);

                Random random = new Random();
                int randomNumber = random.Next();
                String newFileName = prefix + "employee-avatar" + randomNumber + extName;

                AvatarFile.SaveAs(uploadFolderPath + "/" + newFileName);

                // Delete old file
                if (System.IO.File.Exists(uploadFolderPath + "/" + employee.Avatar) && !employee.Avatar.Equals("default-employee-avatar.png"))
                {
                    System.IO.File.Delete(uploadFolderPath + "/" + employee.Avatar);
                }

                employee.Avatar = newFileName;
                employee.UpdatedAt = DateTime.Now;

                context.SaveChanges();

                TempData["Success"] = "Successfully update avatar";
            }

            return RedirectToAction("MyProfile");
        }

        // GET: Admin/Employee/EditMyPassword
        [EmployeeAuthorization]
        public ActionResult EditMyPassword()
        {
            return View();
        }

        // PUT: Admin/Employee/UpdateMyPassword
        [EmployeeAuthorization]
        [HttpPut, ValidateAntiForgeryToken]
        public ActionResult UpdateMyPassword(MyPasswordEdit data)
        {
            if (!ModelState.IsValid) return View("EditMyPassword");

            int ID = AuthManager.CurrentEmployee.GetEmployee.EmployeeID;
            Employee myProfile = context.Employees.FirstOrDefault(e => e.EmployeeID == ID);
            if (myProfile != null && Crypto.VerifyHashedPassword(myProfile.Password, data.CurrentPassword))
            {
                myProfile.Password = Crypto.HashPassword(data.NewPassword);
                myProfile.UpdatedAt = DateTime.Now;

                context.SaveChanges();

                TempData["Success"] = "Successfully update my password";

                return RedirectToAction("MyProfile");
            }
            else
            {
                ModelState.AddModelError("CurrentPassword", "Current password incorrect, type again!");
                return View("EditMyPassword");
            }
        }
    }
}