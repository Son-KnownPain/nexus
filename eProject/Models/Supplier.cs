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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using System.Web;

    public partial class Supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            this.Equipments = new HashSet<Equipment>();
        }
    
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Company name can not empty")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Contact name can not empty")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Address can not empty")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone can not empty")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone only accept 10 number (0-9)")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Fax can not empty")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Phone only accept 10 number (0-9)")]
        public string Fax { get; set; }

        [Required(ErrorMessage = "Contact Url can not empty")]
        public string ContactUrl { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Equipment> Equipments { get; set; }
    }
}
