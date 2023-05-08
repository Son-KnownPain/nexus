using eProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;

namespace eProject.Areas.Admin.Controllers
{
    [AdministratorAuthorization]
    public class CityController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Admin/City
        public ActionResult Index()
        {
            ViewBag.citys = context.Citys.ToList();
            return View();
        }

        // GET: Admin/City/Add
        public ActionResult Add()
        {
            return View();
        }

        // POST: Admin/City/Store
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Store(City data)
        {
            if (!ModelState.IsValid) return View("Add");

            City newCity = new City();
            newCity.CityName = data.CityName;
            newCity.Code = data.Code;

            context.Citys.Add(newCity);

            context.SaveChanges();

            TempData["Success"] = "Successfully add new city";

            return RedirectToAction("Index");
        }

        // GET: Admin/City/Edit
        public ActionResult Edit(int? cityID)
        {
            if (cityID == null) return RedirectToAction("Index");

            City cityModel = context.Citys.FirstOrDefault(c => c.CityID == cityID);

            if (cityModel == null) return RedirectToAction("Index");

            return View(cityModel);
        }

        // PUT: Admin/City/Update
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update(City data)
        {
            if (!ModelState.IsValid) return View("Edit");

            City city = context.Citys.FirstOrDefault(c => c.CityID == data.CityID);

            if (city == null) return RedirectToAction("Index");

            city.CityName = data.CityName;
            city.Code = data.Code;

            context.SaveChanges();

            TempData["Success"] = "Successfully update city information";

            return RedirectToAction("Index");
        }

        // GET: Admin/City/Delete
        public ActionResult Delete(int? cityID)
        {
            if (cityID == null) return RedirectToAction("Index");

            City cityModel = context.Citys.FirstOrDefault(c => c.CityID == cityID);

            if (cityModel == null) return RedirectToAction("Index");

            context.Citys.Remove(cityModel);

            context.SaveChanges();

            TempData["Success"] = "Successfully delete city";

            return RedirectToAction("Index");
        }

        // GET: Admin/City/Search
        public ActionResult Search(string keyword)
        {
            ViewBag.citys = context.Citys.Where(c => c.CityName.Contains(keyword) || c.Code.Contains(keyword)).ToList();
            return View("Index");
        }
    }
}