using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;
using VTA.Buckets.Models;
using VTA.Buckets.Providers;
using VTA.Constants;

namespace VTA.Buckets.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IBucket vehicleBucket;

        public VehicleRepository(IVehicleBucketProvider vehicleBucketProvider)
        {
            vehicleBucket = vehicleBucketProvider.GetBucket();
        }

        public IEnumerable<LocationRecord> GetLocations(string vehicleId, DateTime from, DateTime to)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.LOCATION_RECORD}' and v.vehicleId = '{vehicleId}' and v.time >= {from.Ticks} and v.time <= {to.Ticks} 
                            order by v.time asc";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.Query<LocationRecord>(query);

            return result.Rows;
        }

        public User GetUser(string username)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.USER}' and v.username = '{username}'";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.Query<User>(query);
            return result.Rows.Count > 0 ? result.Rows[0] : null;
        }

        public Vehicle GetVehicleById(string vehicleId)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.VEHICLE}' and v.vehicleId = '{vehicleId}'";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.Query<Vehicle>(query);
            return result.Rows.Count > 0 ? result.Rows[0] : null;
        }

        public async Task<IOperationResult<LocationRecord>> InsertLocationRecordAsync(VTA.Models.Request.LocationRecord locationRecord)
        {
            var documentId = $"{locationRecord.VehicleId}_{locationRecord.Time.Ticks}";
            var result = await vehicleBucket.InsertAsync(documentId, new LocationRecord
            {
                VehicleId = locationRecord.VehicleId,
                Time = locationRecord.Time.Ticks,
                Longtitude = locationRecord.Longitude,
                Latitude = locationRecord.Latitude,
            });
            return result;
        }

        public async Task<IOperationResult<Vehicle>> InsertVehicleAsync(VTA.Models.Request.VehicleRegister vehicleRegister)
        {
            var result = await vehicleBucket.InsertAsync(vehicleRegister.VehicleId, new Vehicle
            {
                VehicleId = vehicleRegister.VehicleId,
                DeviceId = vehicleRegister.DeviceId,
            });

            return result;
        }

        public async Task<IOperationResult<Vehicle>> UpsertVehicleAsync(VTA.Models.Request.LocationRecord locationRecord)
        {
            var result = await vehicleBucket.UpsertAsync(locationRecord.VehicleId, new Vehicle
            {
                VehicleId = locationRecord.VehicleId,
                DeviceId = locationRecord.DeviceId,
                LastLongtitude = locationRecord.Longitude,
                LastLatitude = locationRecord.Latitude,
            });
            return result;
        }

        public bool IsPaired(string vehicleId, string deviceId)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.VEHICLE}' and v.vehicleId = '{vehicleId}' and v.deviceId = '{deviceId}'";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.Query<Vehicle>(query);
            return result.Rows.Count > 0;
        }

        public bool IsRegistered(string vehicleId, string deviceId)
        {
            var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.VEHICLE}' and (v.vehicleId = '{vehicleId}' or v.deviceId = '{deviceId}')";
            var query = QueryRequest.Create(n1sql);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var result = vehicleBucket.Query<Vehicle>(query);
            return result.Rows.Count > 0;
        }
    }
}
