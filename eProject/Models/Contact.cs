//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eProject.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Contact
    {
        public int ContactID { get; set; }
        [DisplayName("Contact Name")]
        [Required(ErrorMessage = "Contact name can not empty")]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "Phone can not empty")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email can not empty")]
        public string Email { get; set; }
        [DisplayName("Message")]
        [Required(ErrorMessage = "Message can not empty")]
        public string Content { get; set; }
        public string Status { get; set; }
        public System.DateTime ContactedAt { get; set; }
    }
}
