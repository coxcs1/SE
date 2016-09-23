using SoftwareEngineering1Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftwareEngineering1Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "The information below will allow you to contact the Department of Computing concerning any questions.";

            return View();
        }
    }
}