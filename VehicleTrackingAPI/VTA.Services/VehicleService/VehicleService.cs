using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTA.Buckets.Repositories;
using VTA.Constants;
using VTA.Models.Request;
using VTA.Models.Response;
using VTA.Services.LocationNameService;

namespace VTA.Services.VehicleService
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository vehicleRepository;

        private readonly ILocationNameService locationNameService;

        public VehicleService(IVehicleRepository vehicleRepository, ILocationNameService locationNameService)
        {
            this.vehicleRepository = vehicleRepository;
            this.locationNameService = locationNameService;
        }

        public Result<bool> RecordLocation(LocationRecord locationRecord)
        {
            if (!vehicleRepository.IsPaired(locationRecord.VehicleId, locationRecord.DeviceId))
            {
                return new Result<bool>(false, Message.VEHICLE_DEVICE_INVALID);
            }

            vehicleRepository.InsertLocationRecordAsync(locationRecord);

            vehicleRepository.UpsertVehicleAsync(locationRecord);

            return new Result<bool>(true);
        }

        public Result<bool> Register(VehicleRegister vehicleRegister)
        {
            if (vehicleRepository.IsRegistered(vehicleRegister.VehicleId, vehicleRegister.DeviceId))
            {
                return new Result<bool>(false, Message.VEHICLE_DEVICE_REGISTER_ALREADY);
            }

            vehicleRepository.InsertVehicleAsync(vehicleRegister);
            
            return new Result<bool>(true);
        }

        public async Task<Result<LocationName>> GetLatestLocationAsync(string vehicleId)
        {
            var vehicle = vehicleRepository.GetVehicleById(vehicleId);
            if (vehicle == null)
            {
                return new Result<LocationName>(null, Message.VEHICLE_NOT_EXISTED);
            }
            else
            {
                if (!vehicle.LastLongtitude.HasValue)
                {
                    return new Result<LocationName>(null, Message.VEHICLE_LOCATION_NOT_RECORDED);
                }
                else
                {
                    var locationName = await locationNameService.GetLocationNameAsync(vehicle.LastLatitude.Value,vehicle.LastLongtitude.Value);

                    return locationName;
                }
            }
        }

        public Result<List<Location>> GetLocations(string vehicleId, DateTime from, DateTime to)
        {
            var locationRecords = vehicleRepository.GetLocations(vehicleId, from, to);
            if (locationRecords.Count() == 0)
            {
                return new Result<List<Location>>(null, Message.NO_LOCATION_RECORDED);
            }
            else
            {
                var data = locationRecords.Select(x => new Location { Longtitude = x.Longtitude, Latitude = x.Latitude}).ToList();
                return new Result<List<Location>>(data);
            }
        }
    }
}
