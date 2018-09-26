using System;
using System.ComponentModel.DataAnnotations;

namespace VTA.Models.Request
{
    public class LocationRecord
    {
        [Required]
        // The id must match with the format : letters, numbers, underscore, dash, point
        [RegularExpression("^[a-zA-Z0-9_.-]*$")]
        public string VehicleId { get; set; }

        [Required]
        // The id must match with the format : letters, numbers, underscore, dash, point
        [RegularExpression("^[a-zA-Z0-9_.-]*$")]
        public string DeviceId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }
    }
}
