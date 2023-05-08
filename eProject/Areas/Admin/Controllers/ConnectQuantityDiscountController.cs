using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using eProject.Auth;
namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class ConnectQuantityDiscountController : Controller
    {
        NexusEntities context = new NexusEntities();
        // GET: Admin/ConnectQuantityDiscount
        public ActionResult Index()
        {
            List<ConnectQuantityDiscount> connectQuantityDiscountList = context.ConnectQuantityDiscounts.ToList();
            ViewBag.connectQuantityDiscountList = connectQuantityDiscountList;
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Store(ConnectQuantityDiscount connectQuantityDiscount)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Add");
            }

            if (context.ConnectQuantityDiscounts.FirstOrDefault(w => w.QuantityFrom == connectQuantityDiscount.QuantityFrom && w.QuantityTo == connectQuantityDiscount.QuantityTo) != null)
            {
                TempData["Error"] = "Exists, Please edit";
                return RedirectToAction("Add");
            }

            ConnectQuantityDiscount newConnectQuantityDiscount = new ConnectQuantityDiscount();
            newConnectQuantityDiscount.DiscountValue = connectQuantityDiscount.DiscountValue;
            newConnectQuantityDiscount.QuantityFrom = connectQuantityDiscount.QuantityFrom;
            newConnectQuantityDiscount.QuantityTo = connectQuantityDiscount.QuantityTo;

            if(connectQuantityDiscount.QuantityTo <= connectQuantityDiscount.QuantityFrom)
            {
                TempData["Error"] = "The number of to connections must be greater than the number of from, Please edit";
                return RedirectToAction("Add");
            }

            context.ConnectQuantityDiscounts.Add(newConnectQuantityDiscount);
            context.SaveChanges();

            TempData["Success"] = "Successfully add connect quantity discount";
            return Redirect("Index");
        }

        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            ConnectQuantityDiscount connectQuantityDiscount = context.ConnectQuantityDiscounts.FirstOrDefault(cqd => cqd.DiscountID == id);
            if(connectQuantityDiscount == null)
            {
                return RedirectToAction("Index");
            }

            context.ConnectQuantityDiscounts.Remove(connectQuantityDiscount);
            context.SaveChanges();

            TempData["Success"] = "Successfully delete connect quantity discount";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? editID)
        {
            if(editID == null)
            {
                return RedirectToAction("Index");
            }

            ConnectQuantityDiscount connectQuantityDiscount = context.ConnectQuantityDiscounts.FirstOrDefault(cqd => cqd.DiscountID == editID);
            if(connectQuantityDiscount == null)
            {
                return RedirectToAction("Index");
            }
            return View(connectQuantityDiscount);
        }

        [HttpPut, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Update(ConnectQuantityDiscount quantityDiscount)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Edit");
            }

            if (context.ConnectQuantityDiscounts.FirstOrDefault(w => w.QuantityFrom == quantityDiscount.QuantityFrom && w.QuantityTo == quantityDiscount.QuantityTo) != null)
            {
                TempData["Error"] = "Exists, Please edit";
                return RedirectToAction("Edit");
            }

            ConnectQuantityDiscount newConnectQuantity = context.ConnectQuantityDiscounts.FirstOrDefault(cqd => cqd.DiscountID == quantityDiscount.DiscountID);
            if(newConnectQuantity != null)
            {
                newConnectQuantity.DiscountValue = quantityDiscount.DiscountValue;
                newConnectQuantity.QuantityFrom = quantityDiscount.QuantityFrom;
                newConnectQuantity.QuantityTo = quantityDiscount.QuantityTo;

                if (quantityDiscount.QuantityTo <= quantityDiscount.QuantityFrom)
                {
                    TempData["Error"] = "The number of to connections must be greater than the number of from, Please edit";
                    return RedirectToAction("Edit");
                }

                context.SaveChanges();

                TempData["Success"] = "Success update connect quantity discount";
            }
            return RedirectToAction("Index");
        }
    }
}