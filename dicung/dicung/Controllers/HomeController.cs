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
            int exist = 0;
            if (db.bookings.Any(x=>x.email == email))
            {
                exist = 1;
            }
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult checkphoneexist(string phone)
        {
            int exist = 0;
            if (db.bookings.Any(x => x.phone == phone))
            {
                exist = 1;
            }
            return Json(exist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult booking(booking model)
        {
            var data = new resultJSON();
            
            if (model != null)
            {
                // checkemail
                //if (db.bookings.Any(x=>x.email == model.email))
                //{
                //    data.success = "0";
                //    data.msb.Add(new MsbList() { field = "booking_email", error = "Email đã được sử dụng." });
                    
                //}
                // check phone
                //if (db.bookings.Any(x => x.phone == model.phone))
                //{
                //    data.success = "0";
                //    data.msb.Add(new MsbList() { field = "booking_phone", error = "Số điện thoại đã được sử dụng." });
                //}

                //if (data.success == "0")
                //{
                //    return Json(data, JsonRequestBehavior.AllowGet);
                //}

                // add data
                booking newbooking = new booking();
                newbooking.full_name = model.full_name ?? null;
                newbooking.phone = model.phone ?? null;
                newbooking.email = model.email ?? null;
                MD5 md5Hash = MD5.Create();
                string passHash = Configs.GetMd5Hash(md5Hash, model.pass);
                newbooking.pass = passHash;
                newbooking.time_go = model.time_go ?? null;
                newbooking.time_to = model.time_to ?? null;
                newbooking.from_location = model.from_location ?? null;
                newbooking.to_location = model.to_location ?? null;
                newbooking.long_from = model.long_from ?? null;
                newbooking.lat_from = model.lat_from ?? null;
                newbooking.long_to = model.long_to ?? null;
                newbooking.lat_to = model.lat_to ?? null;
                newbooking.km1 = model.km1 ?? null;
                newbooking.type = model.type ?? null;
                newbooking.type_vehicle = model.type_vehicle ?? null;
                newbooking.status = 0;
                newbooking.date_time = DateTime.Now;
                newbooking.sex = model.sex ?? null;
                newbooking.group_by = model.group_by ?? null;
                newbooking.group_number = model.group_number ?? null;
                db.bookings.Add(newbooking);
                db.SaveChanges();
                data.success = "1";
                data.msb = null;

            }            


            return Json(data, JsonRequestBehavior.AllowGet);
        }



    }
}