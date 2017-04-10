using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dicung.Models
{
    public class resultJSON
    {
        public string success { get; set; }
        public List<MsbList> msb { get; set; }
    }

    public class MsbList {
        public string field { get; set; }
        public string error { get; set; }
    }


}