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

        // This method is a checkpoint to make sure VehicleId and DeviceId are registered, prevent a device updates location for other vehicles.
        bool IsPaired(string vehicleId, string deviceId);

        Task<Result<LocationName>> GetLatestLocationAsync(string vehicleId);

        Result<List<Location>> GetLocations(string vehicleId, DateTime from, DateTime to);

        // In case replacing device for any reason, such as: damaged, ... , we call the Repair method to re-pair VehicleId and DeviceId.
        // This method is not implemented, just a discussion for another use-case.
        //Result<bool> Repair(string vehicleId, string deviceId);
    }
}
