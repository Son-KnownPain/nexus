using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Models.ViewModels.WarehouseEquipment
{
    public class WarehouseEquipmentViewModel
    {
        public int ID { get; set; }
        [DisplayName("Supplier list")]
        public int WarehouseID { get; set; }
        [DisplayName("Equipment list")]
        public int EquipmentID { get; set; }

        [Required(ErrorMessage = "Number of devices cannot be empty")]
        [RegularExpression("[0-9]+", ErrorMessage = "The number of devices is negative")]
        [Range(1, 9999, ErrorMessage = "The number of devices is neither zero")]
        public int Quantity { get; set; }

        public IEnumerable<SelectListItem> WarehouseList
        {
            get
            {
                NexusEntities context = new NexusEntities();
                return context.Warehouses.Select(w => new SelectListItem
                {
                    Text = w.Name,
                    Value = w.WarehouseID.ToString(),
                }).ToList();
            }
        }

        public IEnumerable<SelectListItem> EquipmentList
        {
            get
            {
                NexusEntities context = new NexusEntities();
                return context.Equipments.Select(e => new SelectListItem
                {
                    Text = e.EquipmentName,
                    Value = e.EquipmentID.ToString(),
                }).ToList();
            }
        }

    }
}