using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PriceWatchEntrySystem.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult DataEntry()
        {
            return View();
        }

        public ActionResult DataVisualization()
        {
            return View();
        }



        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public bool IsUserLoggedIn()
        {
            string str1 = Request.Cookies["CurUsername"].Value;

            if (str1 != null)
                return true;
            else
                return false;
        }
    }
}