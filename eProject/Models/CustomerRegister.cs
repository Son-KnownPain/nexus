using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace eProject.Models
{
    public class CustomerRegister
    {
        [Required(ErrorMessage = "Fullname can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        [DisplayName("Full Name")]
        public string Fullname { get; set; }

        // ------------------------------------------------

        [Required(ErrorMessage = "Phone can not empty")]
        [RegularExpression("[0-9]+", ErrorMessage = "Username only accept number")]
        [StringLength(10, ErrorMessage = "Phone must be 10 numbers")]
        [DisplayName("Phone Number")]
        public string Phone { get; set; }

        // ------------------------------------------------

        [Required(ErrorMessage = "Password can not empty")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Length must be from 6 to 255 characters")]
        public string Password { get; set; }

        // ------------------------------------------------

        [Required(ErrorMessage = "Password confirm can not empty")]
        [Compare("Password", ErrorMessage = "Password confirm is not match")]
        [DisplayName("Password Confirm")]
        public string PasswordConfirm { get; set; }
    }
}