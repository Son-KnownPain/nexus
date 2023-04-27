using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject.Models
{
    public class SimpleProxyEmployee
    {
        [Required(ErrorMessage = "Phone is require")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is require")]
        public string Password { get; set; }
    }
}