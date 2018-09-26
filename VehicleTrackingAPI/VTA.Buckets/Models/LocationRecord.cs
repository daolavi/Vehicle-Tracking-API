using System;
using System.Collections.Generic;
using System.Text;

namespace VTA.Buckets.Models
{
    public class LocationRecord
    {
        public string Type => typeof(LocationRecord).Name;

        public string VehicleId { get; set; }

        public long Time { get; set; }

        public double Longtitude { get; set; }

        public double Latitude { get; set; }
    }
}
