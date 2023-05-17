using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Models.ViewModels.Equipment
{
    public class EquipmentViewModel
    {
        public int EquipmentID { get; set; }

        [Required(ErrorMessage = "Supplier id can not empty")]
        [DisplayName("Supplier List")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Equipment name can not empty")]
        [DisplayName("Equipment Name")]
        public string EquipmentName { get; set; }
        [Required(ErrorMessage = "Description can not empty")]
        [MaxLength(1000, ErrorMessage = "Max length is 1000")]
        public string Description { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase imageFile { get; set; }

        public IEnumerable<SelectListItem> SupplierList
        {
            get
            {
                NexusEntities context = new NexusEntities();
                return context.Suppliers.Select(m => new SelectListItem
                {
                    Text = m.CompanyName,
                    Value = m.SupplierID.ToString(),
                }).ToList();
            }

        }
    }
}