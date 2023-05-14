using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Models.ViewModels.Employee
{
    public class RetailStoreEmployeeDetailViewModel
    {
        public int EmployeeID { get; set; }

        public int RetailShowRoomID { get; set; }
        public IEnumerable<SelectListItem> RetailShowRoomList
        {
            get
            {
                NexusEntities context = new NexusEntities();
                return context.RetailShowRooms.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.RetailShowRoomID.ToString(),
                }).ToList();
            }
        }

        [Required(ErrorMessage = "Full name is require")]
        [DisplayName("Full Name")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Role is require")]
        [Range(1, 10, ErrorMessage = "Value only accept 1 - 10")]
        public int Role { get; set; }

        public IEnumerable<SelectListItem> RoleList
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Patriarch", Value = "6", Selected = true },
                    new SelectListItem { Text = "Vice-Patriarch", Value = "7" },
                    new SelectListItem { Text = "Employees of the store", Value = "5" },
                };
            }
        }

        [Required(ErrorMessage = "Phone is require")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone only accept 10 number (0-9)")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is require")]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is require")]
        [StringLength(26, MinimumLength = 6, ErrorMessage = "Password length from 6 to 26 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirm is require")]
        [DisplayName("Confirm Password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Confirm password doesn't match, type again !")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Address is require")]
        public string Address { get; set; }

        public string Avatar { get; set; }

        public HttpPostedFileBase AvatarFile { get; set; }
    }
}