using Couchbase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTA.Buckets.Models;

namespace VTA.Buckets.Repositories
{
    public interface IVehicleRepository
    {
        User GetUser(string username);

        Vehicle GetVehicleById(string vehicleId);

        IEnumerable<LocationRecord> GetLocations(string vehicleId, DateTime from, DateTime to);

        Task<IOperationResult<LocationRecord>> InsertLocationRecordAsync(VTA.Models.Request.LocationRecord locationRecord);

        Task<IOperationResult<Vehicle>> InsertVehicleAsync(VTA.Models.Request.VehicleRegister registerVehicle);

        Task<IOperationResult<Vehicle>> UpsertVehicleAsync(VTA.Models.Request.LocationRecord locationRecord);

        bool IsRegistered(string vehicleId, string deviceId);

        // This method is used for checking whether VehicleId and DeviceId are registered, preventing a device updates location for other vehicles.
        bool IsPaired(string vehicleId, string deviceId);
    }
}
