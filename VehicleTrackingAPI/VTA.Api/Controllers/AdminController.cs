using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VTA.Services.VehicleService;

namespace VTA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IVehicleService vehicleService;

        public AdminController(IVehicleService vehicleService)
        {
            this.vehicleService = vehicleService;
        }

        [HttpGet]
        [Route("latestlocation")]
        public async Task<IActionResult> GetLatestLocationAsync(string vehicleId)
        {
            var result = await vehicleService.GetLatestLocationAsync(vehicleId);
            return new JsonResult(result);
        }

        [HttpGet]
        [Route("getlocations")]
        public IActionResult GetLocations(string vehicleId, DateTime from, DateTime to)
        {
            var result = vehicleService.GetLocations(vehicleId, from, to);
            return new JsonResult(result);
        }
    }
}
