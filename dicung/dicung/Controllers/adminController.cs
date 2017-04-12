using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dicung.Controllers
{
    public class adminController : Controller
    {
        // GET: admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult listbooking()
        {
            return View();
        }

        public ActionResult listwaybooking()
        {
            return View();
        }
    }
}