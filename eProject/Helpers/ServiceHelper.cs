using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using eProject.Models;

namespace eProject.Helpers
{
    public static class ServiceHelper
    {
        private static NexusEntities context = new NexusEntities();

        public static List<PaymentPlanDetail> GetPldByID(int plID)
        {
            return context.PaymentPlanDetails.Where(a => a.PaymentPlanID == plID).ToList();
        }

        public static List<Service> GetOtherServiceID(int serviceID)
        {
            return context.Services.Where(s => s.ServiceID != serviceID).ToList();
        }

        public static List<Service> GetLv2MenuService()
        {
            return context.Services.ToList();
        }
    }
}