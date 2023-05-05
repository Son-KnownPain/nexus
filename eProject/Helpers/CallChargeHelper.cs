using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eProject.Models.ViewModels.CallCharge;
using System.Web.Mvc;

namespace eProject.Helpers
{
    public static class CallChargeHelper
    {
        private static NexusEntities context = new NexusEntities();

        public static int GetCallChargeCount(int paymentPlanDetailID)
        {
            return context.CallCharges.Where(c => c.PaymentPlanDetailID == paymentPlanDetailID).Count();
        }

        public static List<CallChargeViewModel> GetCallCharge(int pldID)
        {
            return context.CallCharges.Join(context.CallChargeTypes, cc => cc.CallChargeTypeID, cct => cct.CallChargeTypeID, (cc, cct) => new CallChargeViewModel
            {
                CallChargeID = cc.CallChargeID,
                PaymentPlanDetailID = cc.PaymentPlanDetailID,
                CallChargeTypeID = cct.CallChargeTypeID,
                CallChargeName = cct.TypeName,
                CostPerMinute = cc.CostPerMinute,
            }).Where(m => m.PaymentPlanDetailID == pldID).ToList();
        }
    }
}