using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProject.Helpers
{
    public static class CallChargeHelper
    {
        private static NexusEntities context = new NexusEntities();

        public static int GetCallChargeCount(int paymentPlanDetailID)
        {
            return context.CallCharges.Where(c => c.PaymentPlanDetailID == paymentPlanDetailID).Count();
        }
    }
}