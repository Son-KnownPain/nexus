using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.CustomerFeedback
{
    public class CustomerFeedbackViewModel
    {
        [Required(ErrorMessage = "AccountID is require")]
        public string AccountID { get; set; }

        [Required(ErrorMessage = "Content is require")]
        public string Content { get; set; }
    }
}