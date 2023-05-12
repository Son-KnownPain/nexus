using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Models.ViewModels.Order
{
    public class OrderCompleteViewModel
    {
        [Required(ErrorMessage = "Order ID is require")]
        public string OrderID { get; set; }

        [DisplayName("Choose City")]
        [Required(ErrorMessage = "City Code is require")]
        [RegularExpression("[0-9]{3}", ErrorMessage = "City code accept 3 number")]
        public string CityCode { get; set; }
        public IEnumerable<SelectListItem> CityList
        {
            get
            {
                NexusEntities context = new NexusEntities();
                return context.Citys.Select(o => new SelectListItem
                {
                    Text = o.CityName,
                    Value = o.Code
                }).ToList();
            }
        }
    }
}