using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.PaymentPlanDetails
{
    public class PaymentPlanDetailViewModel
    {
        public int PaymentPlanDetailID { get; set; }
        public int PaymentPlanID { get; set; }

        [Required(ErrorMessage = "Content is require")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Rental cost is require")]
        public int RentalCost { get; set; }

        [Required(ErrorMessage = "Vadility days is require")]
        public int VadilityDays { get; set; }
    }
}