using System.ComponentModel.DataAnnotations;

namespace VTA.Models.Request
{
    public class VehicleRegister
    {
        [Required]
        public string VehicleId { get; set; }

        [Required]
        public string DeviceId { get; set; }
    }
}
