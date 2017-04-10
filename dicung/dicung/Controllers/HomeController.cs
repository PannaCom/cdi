using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dicung.Models;
using System.Security.Cryptography;

namespace dicung.Controllers
{
    public class HomeController : Controller
    {
        private dicungDbEntities db = new dicungDbEntities();

        public ActionResult Index()
        {
            return View();
        }

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

        [HttpPost]
        public ActionResult checkemailexist(string email)
        {
            bool exist = false;
            if (db.bookings.Any(x=>x.email == email))
            {
                exist = true;
            }
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult checkphoneexist(string phone)
        {
            bool exist = false;
            if (db.bookings.Any(x => x.phone == phone))
            {
                exist = true;
            }
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult booking(booking model)
        {
            var data = new resultJSON();
            
            if (model != null)
            {
                // checkemail
                if (db.bookings.Any(x=>x.email == model.email))
                {
                    data.success = "0";
                    data.msb.Add(new MsbList() { field = "booking_email", error = "Email đã được sử dụng." });
                    
                }
                // check phone
                if (db.bookings.Any(x => x.phone == model.phone))
                {
                    data.success = "0";
                    data.msb.Add(new MsbList() { field = "booking_phone", error = "Số điện thoại đã được sử dụng." });
                }

                if (data.success == "0")
	            {
		            return Json(data, JsonRequestBehavior.AllowGet);
	            }

                // add data
                booking newbooking = new booking();
                newbooking.full_name = model.full_name ?? null;
                newbooking.phone = model.phone ?? null;
                newbooking.email = model.email ?? null;
                MD5 md5Hash = MD5.Create();
                string passHash = Configs.GetMd5Hash(md5Hash, model.pass);
                newbooking.pass = passHash;
                newbooking.time_go = model.time_go ?? null;
            }            


            return Json(data, JsonRequestBehavior.AllowGet);
        }



    }
}