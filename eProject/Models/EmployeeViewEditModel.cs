using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Models
{
    public class EmployeeViewEditModel
    {
        [Required(ErrorMessage = "Employee ID is require")]
        public int EmployeeID { get; set; }

        public bool StillWorking { get; set; }

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
                    new SelectListItem { Text = "Retail Store Employee", Value = "1", Selected = true },
                    new SelectListItem { Text = "Technical Employee", Value = "2" },
                    new SelectListItem { Text = "Account Employee", Value = "3" },
                    new SelectListItem { Text = "Admin Employee", Value = "10" },
                };
            }
        }

        [Required(ErrorMessage = "Phone is require")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone only accept 10 number (0-9)")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is require")]
        [EmailAddress(ErrorMessage = "Email invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is require")]
        public string Address { get; set; }

        public string Avatar { get; set; }

        public HttpPostedFileBase AvatarFile { get; set; }
    }
}