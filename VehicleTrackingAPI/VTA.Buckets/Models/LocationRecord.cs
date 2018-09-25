using System;
using System.Collections.Generic;
using System.Text;

namespace VTA.Buckets.Models
{
    public class LocationRecord
    {
        public string Type => typeof(LocationRecord).Name;

        public string VehicleId { get; set; }

        public DateTime Time { get; set; }

        public decimal Longtitude { get; set; }

        public decimal Latitude { get; set; }
    }
}
