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
        public string OrderID { get; set; }
        public int ServiceID { get; set; }
        public int CustomerID { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AddressDetail { get; set; }
        public int ConnectQuantity { get; set; }
        public System.DateTime OrderDate { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Service Service { get; set; }
    }
}
