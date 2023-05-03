using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eProject.Models.ViewModels.CallCharge
{
    public class CallChargeViewModel
    {
        public int CallChargeID { get; set; }
        public int PaymentPlanDetailID { get; set; }

        [DisplayName("Call Charge Type")]
        public int CallChargeTypeID { get; set; }

        public string CallChargeName { get; set; }

        [Required(ErrorMessage = "Cost per minute is require")]
        [DisplayName("Cents / 1 Minute")]
        public int CostPerMinute { get; set; }
        public IEnumerable<SelectListItem> CallChargeTypeList
        {
            get
            {
                NexusEntities context = new NexusEntities();
                return context.CallChargeTypes
                .Select(c => new SelectListItem { Text = c.TypeName, Value = c.CallChargeTypeID.ToString() })
                .ToList();
            }
        }
    }
}