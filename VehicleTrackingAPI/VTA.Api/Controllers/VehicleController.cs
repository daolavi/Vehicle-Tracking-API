using Microsoft.AspNetCore.Mvc;
using VTA.Models.Request;
using VTA.Services.VehicleService;

namespace VTA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] VehicleRegister vehicleRegister)
        {
            var result = vehicleService.Register(vehicleRegister);
            return new JsonResult(result);
        }
    }
}
