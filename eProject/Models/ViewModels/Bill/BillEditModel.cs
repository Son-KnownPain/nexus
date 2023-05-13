using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.Bill
{
    public class BillEditModel
    {
        public int BillID { get; set; }
        [DisplayName("Initial Due Amount")]
        [Required(ErrorMessage = "Initial Due Amount is require")]
        public double InitialDueAmount { get; set; }

        [DisplayName("Amount Paid")]
        [Required(ErrorMessage = "Amount Paid is require")]
        public double AmountPaid { get; set; }

        [Required(ErrorMessage = "Discount is require")]
        public double Discount { get; set; }

        [DisplayName("VAT Cost")]
        [Required(ErrorMessage = "Vat Cost is require")]
        public double VatCost { get; set; }

        [DisplayName("Paid Content")]
        [Required(ErrorMessage = "Paid Content is require")]
        public string PaidContent { get; set; }
        public string Status { get; set; }
        public System.DateTime CreatedDate { get; set; }

        // Other properties
        [DisplayName("Is Paid")]
        public bool Paid { get; set; }
    }
}