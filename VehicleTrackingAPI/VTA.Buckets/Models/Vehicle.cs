using Couchbase.Linq.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTA.Buckets.Models
{
    [DocumentTypeFilter("Vehicle")]
    public class Vehicle
    {
        public Vehicle()
        {
            Type = typeof(Vehicle).Name;
        }

        public string Type { get; set; }

        public string DeviceId { get; set; }

        public decimal Long { get; set; }

        public decimal Lat { get; set; }
    }
}
