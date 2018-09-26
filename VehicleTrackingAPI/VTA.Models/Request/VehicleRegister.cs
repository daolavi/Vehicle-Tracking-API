using System.ComponentModel.DataAnnotations;

namespace VTA.Models.Request
{
    public class VehicleRegister
    {
        [Required]
        // The id must match with the format : letters, numbers, underscore, dash, point
        [RegularExpression("^[a-zA-Z0-9_.-]*$")]
        public string VehicleId { get; set; }

        [Required]
        // The id must match with the format : letters, numbers, underscore, dash, point
        [RegularExpression("^[a-zA-Z0-9_.-]*$")]
        public string DeviceId { get; set; }
    }
}
