using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using eProject.Models;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class CallChargeTypeController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/CallChargeType
        public ActionResult Index()
        {
            ViewBag.callChargeTypeList = context.CallChargeTypes.ToList();
            return View();
        }

        // POST: Admin/CallChargeType/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Store(CallChargeType data)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.callChargeTypeList = context.CallChargeTypes.ToList();
                return View("Index");
            }

            CallChargeType callChargeType = new CallChargeType();

            callChargeType.TypeName = data.TypeName;

            context.CallChargeTypes.Add(callChargeType);

            context.SaveChanges();

            TempData["Success"] = "Successfully add new call charge type";

            return RedirectToAction("Index");
        }

        // GET: Admin/CallChargeType/Edit
        public ActionResult Edit(int? callChargeTypeID)
        {
            if (callChargeTypeID == null) return RedirectToAction("Index");

            CallChargeType callChargeType = context.CallChargeTypes.FirstOrDefault(c => c.CallChargeTypeID == callChargeTypeID);

            if (callChargeType == null) return RedirectToAction("Index");

            return View(callChargeType);
        }

        // PUT: Admin/CallChargeType/Update
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CallChargeType data)
        {
            if (!ModelState.IsValid) return View("Edit");

            CallChargeType callChargeType = context.CallChargeTypes.FirstOrDefault(c => c.CallChargeTypeID == data.CallChargeTypeID);

            if (callChargeType == null) return RedirectToAction("Index");

            callChargeType.TypeName = data.TypeName;

            context.SaveChanges();

            TempData["Success"] = "Successfully update call charge type";

            return RedirectToAction("Index");
        }

        // GET: Admin/CallChargeType/Delete
        public ActionResult Delete(int? callChargeTypeID)
        {
            if (callChargeTypeID == null)
            {
                return RedirectToAction("Index");
            }

            CallChargeType callChargeType = context.CallChargeTypes.FirstOrDefault(c => c.CallChargeTypeID == callChargeTypeID);
            if (callChargeType == null)
            {
                return RedirectToAction("Index");
            }

            context.CallCharges.RemoveRange(context.CallCharges.Where(c => c.CallChargeTypeID == callChargeTypeID));
            

            context.CallChargeTypes.Remove(callChargeType);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete call charge type";
            return RedirectToAction("Index");
        }
    }
}