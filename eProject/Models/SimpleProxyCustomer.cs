using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eProject.Models
{
    public class SimpleProxyCustomer
    {
        [Required(ErrorMessage = "Phone can not empty")]
        [RegularExpression("[0-9]+", ErrorMessage = "Username only accept number")]
        [StringLength(10, ErrorMessage = "Phone must be 10 numbers")]
        public string Phone { get; set; }

        // ------------------------------------------------

        [Required(ErrorMessage = "Password can not empty")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Length must be from 6 to 255 characters")]
        public string Password { get; set; }
    }
}