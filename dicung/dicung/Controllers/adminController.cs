using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using dicung.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.Entity;

namespace dicung.Controllers
{
    public class adminController : Controller
    {
        private dicungDbEntities db = new dicungDbEntities();
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

        #region ### module get place nearby

        public class mapdata
        {
            public results[] results { get; set; }
        }

        public class nearby
        {
            public results2[] results { get; set; }
        }

        public class photos
        {
            public int? height { get; set; }
            public string[] html_attributions { get; set; }
            public int? width { get; set; }

        }


        public class results
        {
            public geometry geometry { get; set; }
            public string formatted_address { get; set; }
        }

        public class results2
        {
            public geometry geometry { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public photos[] photos { get; set; }
            public double? rating { get; set; }
            public string scope { get; set; }
            public string[] types { get; set; }
            public string vicinity { get; set; }
        }

        public class geometry
        {
            public location location { get; set; }
        }

        public class location
        {
            public double? lat { get; set; }
            public double? lng { get; set; }
        }

        public async Task<ActionResult> getnearbyrestaurent(string type)
        {
            //if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            if (type == null)
            {
                return View();
            }

            List<country> data = new List<country>();

            // get config 
            int? stepid = 0;
            var config = (from c in db.country_config where c.type == type select c).FirstOrDefault();
            if (config != null)
            {
                stepid = config.country_end;
            }

            data = (from s in db.countries orderby s.id where s.lon != null && s.lat != null && s.id > stepid select s).ToList();

            var total = 0;
            var countend = 0;
            var totalplace = 0;
            if (data.Count == 0)
            {
                TempData["Updated"] = "Đã cập nhật danh sách " + type + " cho các tỉnh thành.";
                return View();
            }

            data = data.Take(50).ToList();
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    foreach (var item in data)
                    {
                        var urlreq = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + item.lat + "," + item.lon + "&radius=15000&type=" + type + "&key=AIzaSyBqxfIx32ftFpmqW4i2bZ5bckYM_LBl870";
                        HttpResponseMessage response = await httpClient.GetAsync(urlreq);
                        response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            var stateInfo = response.Content.ReadAsStringAsync().Result;
                            //var geodata = JsonConvert.DeserializeObject<mapdata>(stateInfo);
                            var nearby = await JsonConvert.DeserializeObjectAsync<nearby>(stateInfo);

                            if (nearby != null)
                            {
                                foreach (var itemnearby in nearby.results)
                                {
                                    //check nếu đã có địa điểm đã tồn tại thì bỏ qua, nhảy tới cái tiếp theo
                                    if (db.country_place_nearby.Any(o => o.lat == itemnearby.geometry.location.lat && o.lng == itemnearby.geometry.location.lng)) continue;

                                    country_place_nearby addnearby = new country_place_nearby();
                                    addnearby.country_id = item.id;
                                    addnearby.icon = itemnearby.icon ?? null;
                                    addnearby.lat = itemnearby.geometry.location.lat ?? null;
                                    addnearby.lng = itemnearby.geometry.location.lng ?? null;
                                    addnearby.name = itemnearby.name ?? null;
                                    if (itemnearby.photos != null)
                                    {
                                        addnearby.photo_height = itemnearby.photos[0].height ?? null;
                                        addnearby.photo_width = itemnearby.photos[0].width ?? null;
                                        addnearby.photo_html = itemnearby.photos[0].html_attributions != null ? string.Join(";", itemnearby.photos[0].html_attributions).ToString() : null;
                                    }
                                    addnearby.rating = itemnearby.rating ?? null;
                                    addnearby.scope = itemnearby.scope ?? null;
                                    addnearby.type = type ?? null;
                                    addnearby.types = itemnearby.types != null ? string.Join(";", itemnearby.types) : null;
                                    addnearby.vicinity = itemnearby.vicinity ?? null;
                                    db.country_place_nearby.Add(addnearby);
                                    await db.SaveChangesAsync();
                                    totalplace += 1;
                                }

                                total += 1;
                                countend = item.id;
                            }

                        }
                    }

                }

                //Đánh dấu quan huyen đã quét vào country_config
                if (config != null)
                {
                    config.country_end = countend;
                    db.Entry(config).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    country_config newconfig = new country_config();
                    newconfig.type = type ?? null;
                    newconfig.country_end = countend;
                    db.country_config.Add(newconfig);
                    await db.SaveChangesAsync();
                }

                ViewBag.next = "ok";
                ViewBag.type = type;

            }
            catch (Exception ex)
            {
                Configs.SaveTolog(ex.ToString());
                return View();
            }

            TempData["Updated"] = "Đã lấy được danh sách  <b>" + totalplace + "</b> " + type + "/<b>" + total + "</b> quận huyện, quận huyện id cuối cùng: " + countend;
            return View();
        }

        public async Task<ActionResult> getlonglatquanhuyen(string type)
        {
            //if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Home");
            if (type == null)
            {
                return View();
            }

            // get danh sach quan huyen
            var data = (from s in db.countries where s.lon == null && s.lat == null select s).ToList();
            var total = 0;
            if (data.Count == 0)
            {
                TempData["Updated"] = "Đã lấy hết vị trí lonlat cho các quận huyện.";
                return View();
            }
            try
            {

                using (HttpClient httpClient = new HttpClient())
                {
                    foreach (var item in data)
                    {
                        var urlreq = "https://maps.googleapis.com/maps/api/geocode/json?address=" + item.quanhuyen + "," + item.tinhthanh + "&sensor=false";
                        HttpResponseMessage response = await httpClient.GetAsync(urlreq);
                        response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            var stateInfo = response.Content.ReadAsStringAsync().Result;
                            //var geodata = JsonConvert.DeserializeObject<mapdata>(stateInfo);
                            var geodata = await JsonConvert.DeserializeObjectAsync<mapdata>(stateInfo);

                            if (geodata != null)
                            {
                                //update lonlat quan huyen
                                var _location = geodata.results[0].geometry.location;
                                var _fulladdress = geodata.results[0].formatted_address;

                                item.lat = _location.lat ?? null;
                                item.lon = _location.lng ?? null;
                                item.formatted_address = _fulladdress ?? null;
                                db.Entry(item).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                                total += 1;
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Configs.SaveTolog(ex.ToString());
                return View();
            }


            TempData["Updated"] = "Đã lấy được vị trí lonlat cho: " + total + " quận huyện.";
            return View();
        }

        #endregion

        public class people
        {
            public string full_name { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public TimeSpan? time_go { get; set; }
            public TimeSpan? time_to { get; set; }
            public string from_location { get; set; }
            public double? long_from { get; set; }
            public double? lat_from { get; set; }
            public string to_location { get; set; }
            public double? long_to { get; set; }
            public double? lat_to { get; set; }
            public double? km1 { get; set; }
        }
        public string calculate_for_car()
        {
            try
            {
                people[] pp = null;
                people pt = new people();
                var p = (from q in db.bookings where q.type_vehicle.Contains("DRIVING") && q.group_id == null orderby q.km1 select q).ToList();
                pp = new people[p.Count];
                int i=0;
                for (i = 0; i < p.Count; i++)
                {
                    pt = new people();
                    pt.email = p[i].email;
                    pt.from_location = p[i].from_location;
                    pt.full_name = p[i].full_name;
                    pt.km1 = p[i].km1;
                    pt.lat_from = p[i].lat_from;
                    pt.lat_to = p[i].lat_to;
                    pt.long_from = p[i].long_from;
                    pt.long_to = p[i].long_to;
                    pt.phone = p[i].phone;
                    pt.time_go = p[i].time_go;
                    pt.time_to = p[i].time_to;
                    pt.to_location = p[i].to_location;
                    pp[i] = new people();
                    pp[i] = pt;
                }
                int[] tt = new int[p.Count];
                int group = 0;
                //int abc = 0;
                for (i = 0; i < p.Count-1; i++)
                { 
                    for (var j = i+1; j < p.Count; j++)
                    if (tt[j] == 0 && dt(pp[i], pp[j])&& dtt(pp[i], pp[j]))                    
                    {
                        if (tt[i] == 0)
                        {
                            group++;
                            tt[i] = group;
                            tt[j] = group;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return "1";
        }
        //ACOS(SIN(PI()*" + lat + "/180.0)*SIN(PI()*lat1/180.0)+COS(PI()*" + lat + "/180.0)*COS(PI()*lat1/180.0)*COS(PI()*lon1/180.0-PI()*" + lon + "/180.0))*6371 
        public bool dt(people p1, people p2)
        {
            double x1 =(double)(Math.PI * p1.lat_from) / 180;
            double x2 = (double)(Math.PI * p2.lat_from) / 180;
            double x3 = (double)(Math.PI * p1.lat_from / 180);
            double x4 = (double)(Math.PI * p2.lat_from / 180);
            double x5 = (double)(Math.PI * p2.long_from / 180);
            double x6 = (double)(Math.PI * p1.long_from / 180);           
            bool true1 = Math.Acos(Math.Sin(x1) * Math.Sin(x2) + Math.Cos(x3) * Math.Cos(x4) * Math.Cos(x5 - x6)) * 6371<3;
            x1 = (double)(Math.PI * p1.lat_to) / 180;
            x2 = (double)(Math.PI * p2.lat_to) / 180;
            x3 = (double)(Math.PI * p1.lat_to / 180);
            x4 = (double)(Math.PI * p2.lat_to / 180);
            x5 = (double)(Math.PI * p2.long_to / 180);
            x6 = (double)(Math.PI * p1.long_to / 180); 

            bool true2 = Math.Acos(Math.Sin(x1) * Math.Sin(x2) + Math.Cos(x3) * Math.Cos(x4) * Math.Cos(x5 - x6)) * 6371 < 3;

            return true1 & true2;
        }
        public bool dtt(people p1, people p2)
        {
            //Giờ đi phải sát nhau
            TimeSpan dt =new TimeSpan(p2.time_go.Value.Ticks - p1.time_go.Value.Ticks);
            bool true1 = Math.Abs(dt.TotalMinutes) <= 60;
            //Giờ về phải sát nhau
            dt = dt = new TimeSpan(p2.time_to.Value.Ticks - p1.time_to.Value.Ticks);
            bool true2 = Math.Abs(dt.TotalMinutes) <= 60;
            return true1 & true2;
        }
    }
}