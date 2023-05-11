using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.Account
{
    public class AccountViewModel
    {
        public string AccountID { get; set; }
        // Customer
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        // Payment Plan Detail
        public int PaymentPlanDetailID { get; set; }

        // Service
        public string ServiceName { get; set; }
        public string Thumbnail { get; set; }

        // Order
        public string OrderID { get; set; }
        public string Status { get; set; }
        public string ContactNumber { get; set; }
        public int ConnectQuantity { get; set; }
        public DateTime DueDate { get; set; }
        public System.DateTime ConnectedAt { get; set; }
    }
}