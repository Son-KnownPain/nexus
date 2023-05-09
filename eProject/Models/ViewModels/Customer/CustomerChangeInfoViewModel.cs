using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.Customer
{
    public class CustomerChangeInfoViewModel
    {
        public int CustomerID { get; set; }

        [DisplayName("Fullname")]
        [Required(ErrorMessage = "Fullname can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Phone can not empty")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone only accept 10 number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string Address { get; set; }

        [DisplayName("Address Detail")]
        [Required(ErrorMessage = "Address Detail can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string AddressDetail { get; set; }
    }
}