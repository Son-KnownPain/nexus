using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.CustomerFeedback
{
    public class FeedbackReplyModel
    {
        [Required(ErrorMessage = "CustomerFeedbackID cannot be empty")]
        public int CustomerFeedbackID { get; set; }
        public string AccountID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string Content { get; set; }

        [Required(ErrorMessage = "Reply content cannot be empty")]
        [DisplayName("Reply Content")]
        public string ReplyContent { get; set; }
        public System.DateTime FeedbackAt { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
    }
}