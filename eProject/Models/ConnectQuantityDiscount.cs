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

    public partial class ConnectQuantityDiscount
    {
        public int DiscountID { get; set; }


        [Required(ErrorMessage = "Discount value value cannot be empty")]
        [Range(0, 100, ErrorMessage = "Value from 0 to 100 percent")]
        [DisplayName("Discount Value")]
        public double DiscountValue { get; set; }

        [Required(ErrorMessage = "Quantity from cannot be empty")]
        [RegularExpression("[0-9]+", ErrorMessage = "Quantity from is not a negative number.")]
        [Range(1, 9999, ErrorMessage = "Quantity from not equal to 0.")]
        [DisplayName("Quantity From")]
        public int QuantityFrom { get; set; }

        [Required(ErrorMessage = "Quantity to cannot be empty.")]
        [RegularExpression("[0-9]+", ErrorMessage = "Quantitaty to is not a negative number.")]
        [Range(1, 9999, ErrorMessage = "Quantity to not equal to 0.")]
        [DisplayName("Quantity To")]
        public int QuantityTo { get; set; }
    }
}
