using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Models.ViewModels.Order
{
    public class OrderRegisterViewModel
    {
        // Order
        public string OrderID { get; set; }

        [Required(ErrorMessage = "ServiceID can not empty")]
        public int ServiceID { get; set; }

        [Required(ErrorMessage = "PaymentPlanDetailID can not empty")]
        public int PaymentPlanDetailID { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Phone can not empty")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone only accept 10 number")]
        public string Phone { get; set; }

        //public IEnumerable<SelectListItem> CityList
        //{
        //    get
        //    {
        //        NexusEntities context = new NexusEntities();
        //        return context.Citys.Select(c => new SelectListItem
        //        {
        //            Text = c.CityName,
        //            Value = c.Code
        //        }).ToList();
        //    }
        //}

        //[Required(ErrorMessage = "City Code is require")]
        //[DisplayName("City Code")]
        //public string CityCode { get; set; }

        [Required(ErrorMessage = "Address can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string Address { get; set; }

        [DisplayName("Address Detail")]
        [Required(ErrorMessage = "Address Detail can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string AddressDetail { get; set; }

        [DisplayName("Connect Quantity")]
        [Required(ErrorMessage = "Connect Quantity can not empty")]
        [Range(1, 100, ErrorMessage = "Value must be from 1 to 100")]
        public int ConnectQuantity { get; set; }

        // Customer

        [DisplayName("Fullname")]
        [Required(ErrorMessage = "Fullname can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email can not empty")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is require")]
        [StringLength(26, MinimumLength = 6, ErrorMessage = "Password length from 6 to 26 characters")]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Password confirm is require")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Confirm password doesn't match, type again !")]
        public string PasswordConfirm { get; set; }
    }
}