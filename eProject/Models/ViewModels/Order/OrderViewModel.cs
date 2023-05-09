using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject.Models.ViewModels.Order
{
    public class OrderViewModel
    {
        // Customer
        public int CustomerID { get; set; }
        // Payment Plan Detail
        public int PaymentPlanDetailID { get; set; }
        public string Content { get; set; }

        // Service
        public string ServiceName { get; set; }
        public string Thumbnail { get; set; }

        // Order
        public string OrderID { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AddressDetail { get; set; }
        public int ConnectQuantity { get; set; }
        public int Deposit { get; set; }
        public double DepositDiscount { get; set; }
        public System.DateTime OrderDate { get; set; }
    }
}