//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace dicung.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class country_place_nearby
    {
        public long id { get; set; }
        public Nullable<int> country_id { get; set; }
        public string types { get; set; }
        public string type { get; set; }
        public Nullable<double> lat { get; set; }
        public Nullable<double> lng { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public Nullable<int> photo_height { get; set; }
        public Nullable<int> photo_width { get; set; }
        public string photo_html { get; set; }
        public Nullable<double> rating { get; set; }
        public string scope { get; set; }
        public string vicinity { get; set; }
    }
}
