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
    using System.Web;

    public partial class Service
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Service()
        {
            this.PaymentPlans = new HashSet<PaymentPlan>();
            this.Accounts = new HashSet<Account>();
            this.Orders = new HashSet<Order>();
        }

        public int ServiceID { get; set; }

        [DisplayName("Service Name")]
        [Required(ErrorMessage = "Service name can not empty")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Deposit can not empty")]
        public Nullable<int> Deposit { get; set; }

        [Required(ErrorMessage = "Summary can not empty")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Description can not empty")]
        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public HttpPostedFileBase ThumbnailFile { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentPlan> PaymentPlans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account> Accounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
