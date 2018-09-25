using Couchbase.N1QL;
using VTA.Buckets.Buckets.VehicleBucket;
using VTA.Buckets.Models;
using VTA.Constants;
using VTA.Models.Request;
using VTA.Models.Response;

namespace VTA.Services.VehicleService
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleBucketProvider vehicleBucket;

        public VehicleService(IVehicleBucketProvider vehicleBucket)
        {
            this.vehicleBucket = vehicleBucket;
        }

        public Result<bool> RecordLocation(Models.Request.LocationRecord locationRecord)
        {
            if (!IsPaired(locationRecord.VehicleId, locationRecord.DeviceId))
            {
                return new Result<bool>(false, Message.VEHICLE_DEVICE_INVALID);
            }

            var documentId = $"{locationRecord.VehicleId}_{locationRecord.Time.Ticks}";
            vehicleBucket.GetBucket().Insert(documentId, new Buckets.Models.LocationRecord
            {
                VehicleId = locationRecord.VehicleId,
                Time = locationRecord.Time,
                Longtitude = locationRecord.Longitude,
                Latitude = locationRecord.Latitude,
            });

            vehicleBucket.GetBucket().Upsert(locationRecord.VehicleId, new Vehicle
            {
                VehicleId = locationRecord.VehicleId,
                DeviceId = locationRecord.DeviceId,
                LastLongtitude = locationRecord.Longitude,
                LastLatitude = locationRecord.Latitude,
            });

            return new Result<bool>(true);
        }

        public Result<bool> Register(VehicleRegister registerVehicle)
        {
            if (IsRegistered(registerVehicle.VehicleId, registerVehicle.DeviceId))
            {
                return new Result<bool>(false, Message.VEHICLE_DEVICE_REGISTER_ALREADY);
            }

            vehicleBucket.GetBucket().Insert(registerVehicle.VehicleId, new Vehicle
            {
                VehicleId = registerVehicle.VehicleId,
                DeviceId = registerVehicle.DeviceId,
            });
            
            return new Result<bool>(true);
        }

        public bool IsRegistered(string vehicleId, string deviceId)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.VEHICLE}' and (v.vehicleId = '{vehicleId}' or v.deviceId = '{deviceId}')";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.GetBucket().Query<Vehicle>(query);
            return result.Rows.Count > 0;
        }

        public bool IsPaired(string vehicleId, string deviceId)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.VEHICLE}' and v.vehicleId = '{vehicleId}' and v.deviceId = '{deviceId}'";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.GetBucket().Query<Vehicle>(query);
            return result.Rows.Count > 0;
        }
    }
}
