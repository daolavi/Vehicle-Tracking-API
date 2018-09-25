using System;
using System.ComponentModel.DataAnnotations;

namespace VTA.Models.Request
{
    public class LocationRecord
    {
        [Required]
        public string VehicleId { get; set; }

        [Required]
        public string DeviceId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        public decimal Latitude { get; set; }
    }
}
