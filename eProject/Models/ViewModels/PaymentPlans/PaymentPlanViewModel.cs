using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProject.Models.ViewModels.PaymentPlans
{
    public class PaymentPlanViewModel
    {
        public int PaymentPlanID { get; set; }

        [Required(ErrorMessage = "Service is require")]
        [DisplayName("Service")]
        public int ServiceID { get; set; }

        public string ServiceName { get; set; }

        [DisplayName("Payment Plan Name")]
        [Required(ErrorMessage = "Payment Plan Name is require")]
        [MaxLength(255, ErrorMessage = "Max length is 255 characters")]
        public string PlanName { get; set; }

        [DisplayName("Description For Payment Plan")]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> ServiceList
        {
            get
            {
                NexusEntities context = new NexusEntities();
                return context.Services
                .Select(s => new SelectListItem { Text = s.ServiceName, Value = s.ServiceID.ToString() })
                .ToList();
            }
        }
    }
}