﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eProject.Filters;
using eProject.Models;

namespace eProject.Controllers
{
    public class SiteController : Controller
    {
        private NexusEntities context = new NexusEntities();

        // GET: Site
        public ActionResult Index()
        {
            ViewBag.services = context.Services.ToList();
            return View();
        }

        public ActionResult HowToOrderAService()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }
    }
}