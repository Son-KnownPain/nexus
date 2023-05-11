using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.Customer
{
    public class CustomerPasswordEditViewModel
    {
        [Required(ErrorMessage = "CustomerID is require")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "New password is require")]
        [DisplayName("New Password")]
        [StringLength(26, MinimumLength = 6, ErrorMessage = "New password length from 6 to 26 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "New password confirm is require")]
        [DisplayName("Confirm New Password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Confirm new password doesn't match, type again !")]
        public string NewPasswordConfirm { get; set; }
    }
}