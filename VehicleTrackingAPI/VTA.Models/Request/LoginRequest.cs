using System.ComponentModel.DataAnnotations;

namespace VTA.Models.Request
{
    public class LoginRequest
    {
        [Required]
        // The username must match with the format : letters, numbers, underscore, dash, point
        [RegularExpression("^[a-zA-Z0-9_.-]*$")]
        public string Username { get; set; }

        [Required]
        // The password must match with the format : letters, numbers, underscore, dash, point
        [RegularExpression("^[a-zA-Z0-9_.-]*$")]
        public string Password { get; set; }
    }
}
