using System;
using System.Collections.Generic;
using System.Linq;
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
                Time = locationRecord.Time.Ticks,
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

        public Result<Location> GetLatestLocation(string vehicleId)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.VEHICLE}' and v.vehicleId = '{vehicleId}'";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.GetBucket().Query<Vehicle>(query);
            if (result.Rows.Count == 0)
            {
                return new Result<Location>(null, Message.VEHICLE_NOT_EXISTED);
            }
            else
            {
                var vehicle = result.Rows[0];
                if (!vehicle.LastLongtitude.HasValue)
                {
                    return new Result<Location>(null, Message.VEHICLE_LOCATION_NOT_RECORDED);
                }
                else
                {
                    return new Result<Location>(new Location
                    {
                        Longtitude = vehicle.LastLongtitude.Value,
                        Latitude = vehicle.LastLatitude.Value
                    });
                }
            }
        }

        public Result<List<Location>> GetLocations(string vehicleId, DateTime from, DateTime to)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.LOCATION_RECORD}' and v.vehicleId = '{vehicleId}' and v.time >= {from.Ticks} and v.time <= {to.Ticks} ";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.GetBucket().Query<Buckets.Models.LocationRecord>(query);
            if (result.Rows.Count == 0)
            {
                return new Result<List<Location>>(null, Message.NO_LOCATION_RECORDED);
            }
            else
            {
                var locationRecords = result.Rows;
                var data = locationRecords.Select(x => new Location { Longtitude = x.Longtitude, Latitude = x.Latitude}).ToList();
                return new Result<List<Location>>(data);
            }
        }
    }
}
