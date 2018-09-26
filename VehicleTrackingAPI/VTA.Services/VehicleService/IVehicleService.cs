using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTA.Models.Request;
using VTA.Models.Response;

namespace VTA.Services.VehicleService
{
    public interface IVehicleService
    {
        Result<bool> Register(VehicleRegister registerVehicle);

        Result<bool> RecordLocation(LocationRecord recordPosition);

        bool IsRegistered(string vehicleId, string deviceId);

        bool IsPaired(string vehicleId, string deviceId);

        Task<Result<LocationName>> GetLatestLocationAsync(string vehicleId);

        Result<List<Location>> GetLocations(string vehicleId, DateTime from, DateTime to);
    }
}
