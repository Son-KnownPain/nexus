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
    
    public partial class ConnectionPaymentPlan
    {
        public int ID { get; set; }
        public int ConnectionID { get; set; }
        public int PlanDetailID { get; set; }
        public System.DateTime DueDate { get; set; }
        public int DueAmount { get; set; }
    
        public virtual Connection Connection { get; set; }
        public virtual PlanDetail PlanDetail { get; set; }
    }
}