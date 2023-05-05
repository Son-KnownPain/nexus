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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.Accounts = new HashSet<Account>();
        }
    
        public string OrderID { get; set; }
        public int ServiceID { get; set; }
        public int CustomerID { get; set; }
        public int PaymentPlanDetailID { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AddressDetail { get; set; }
        public int ConnectQuantity { get; set; }
        public int Deposit { get; set; }
        public double DepositDiscount { get; set; }
        public System.DateTime OrderDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual PaymentPlanDetail PaymentPlanDetail { get; set; }
        public virtual Service Service { get; set; }
    }
}
