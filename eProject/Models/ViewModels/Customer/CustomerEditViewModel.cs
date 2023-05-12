using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.Customer
{
    public class CustomerEditViewModel
    {
        [Required(ErrorMessage = "Customer ID is require")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Fullname is require")]
        [DisplayName("Full Name")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Phone is require")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone only accept 10 number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is require")]
        [EmailAddress(ErrorMessage = "That is not email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is require")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Address detail is require")]
        [DisplayName("Address Detail")]
        public string AddressDetail { get; set; }
        public string Avatar { get; set; }
        public HttpPostedFileBase AvatarFile { get; set; }
    }
}