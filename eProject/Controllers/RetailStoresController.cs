using eProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace eProject.Controllers
{
    public class RetailStoresController : Controller
    {

        private NexusEntities context = new NexusEntities();

        // GET: RetailStores
        public ActionResult Index()
        {
            List<RetailShowRoom> retailShowRoomList = context.RetailShowRooms.ToList();
            ViewBag.RetailShowRooms = retailShowRoomList;
            return View();
        }

    }
}