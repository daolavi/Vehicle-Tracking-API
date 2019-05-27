using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTA.Models.Request;
using VTA.Services.VehicleService;

namespace VTA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Use Authorize attribute, in case we want to authenticate requests for VehicleController, such as: clients need to pass a valid token to consume api
    //[Authorize]
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

        [HttpPost]
        [Route("record")]
        public IActionResult Record([FromBody] LocationRecord locationRecord)
        {
            var result = vehicleService.RecordLocation(locationRecord);
            return new JsonResult(result);
        }
    }
}
