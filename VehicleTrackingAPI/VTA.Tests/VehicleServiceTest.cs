using FakeItEasy;
using System;
using System.Collections.Generic;
using VTA.Buckets.Models;
using VTA.Buckets.Repositories;
using VTA.Constants;
using VTA.Models.Request;
using VTA.Models.Response;
using VTA.Services.LocationNameService;
using VTA.Services.VehicleService;
using Xunit;

namespace VTA.Tests
{
    public class VehicleServiceTest
    {
        private readonly IVehicleRepository vehicleRepository;

        private readonly ILocationNameService locationNameService;

        public VehicleServiceTest()
        {
            vehicleRepository = A.Fake<IVehicleRepository>();
            locationNameService = A.Fake<ILocationNameService>();
        }

        [Fact]
        public void RecordLocation_Success()
        {
            A.CallTo(() => vehicleRepository.IsPaired(A<string>._, A<string>._)).Returns(true);
            var locationRecord = new Models.Request.LocationRecord
            {
                VehicleId = "v1",
                DeviceId = "d1",
                Time = DateTime.Now,
                Longitude = 1,
                Latitude = 1,
            };

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.RecordLocation(locationRecord);

            Assert.True(result.Data);
        }

        [Fact]
        public void RecordLocation_Fail()
        {
            A.CallTo(() => vehicleRepository.IsPaired(A<string>._, A<string>._)).Returns(false);
            var locationRecord = new Models.Request.LocationRecord
            {
                VehicleId = "v1",
                DeviceId = "d1",
                Time = DateTime.Now,
                Longitude = 1,
                Latitude = 1,
            };

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.RecordLocation(locationRecord);

            Assert.True(result.IsError);
            Assert.Equal(result.ErrorMessage, Message.VEHICLE_DEVICE_INVALID);
        }

        [Fact]
        public void Register_Success()
        {
            A.CallTo(() => vehicleRepository.IsRegistered(A<string>._, A<string>._)).Returns(false);
            var vehicleRegister = new VehicleRegister
            {
                VehicleId = "v1",
                DeviceId = "d1"
            };

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.Register(vehicleRegister);

            Assert.True(result.Data);
        }

        [Fact]
        public void Register_Fail()
        {
            A.CallTo(() => vehicleRepository.IsRegistered(A<string>._, A<string>._)).Returns(true);
            var vehicleRegister = new VehicleRegister
            {
                VehicleId = "v1",
                DeviceId = "d1"
            };

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.Register(vehicleRegister);

            Assert.True(result.IsError);
            Assert.Equal(result.ErrorMessage, Message.VEHICLE_DEVICE_REGISTER_ALREADY);
        }

        [Fact]
        public void GetLatestLocationAsync_HasData()
        {
            var vehicle = new Vehicle
            {
                VehicleId = "v1",
                DeviceId = "d1",
                LastLongtitude = 1,
                LastLatitude = 1,
            };

            var locationName = new LocationName
            {
                Address = "Bangkok, Thailand",
                Latitude = 1,
                Longtitude = 1,
            };

            A.CallTo(() => vehicleRepository.GetVehicleById(A<string>._)).Returns(vehicle);
            A.CallTo(() => locationNameService.GetLocationNameAsync(A<double>._, A<double>._)).Returns(new Result<LocationName>(locationName));

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.GetLatestLocationAsync(vehicle.VehicleId).Result;

            Assert.False(result.IsError);
            Assert.Equal(result.Data.Address, locationName.Address);
        }

        [Fact]
        public void GetLatestLocationAsync_NoData()
        {
            var vehicle = new Vehicle
            {
                VehicleId = "v1",
                DeviceId = "d1",
                LastLongtitude = null,
                LastLatitude = null,
            };

            var locationName = new LocationName
            {
                Address = "Bangkok, Thailand",
                Latitude = 1,
                Longtitude = 1,
            };

            A.CallTo(() => vehicleRepository.GetVehicleById(A<string>._)).Returns(vehicle);
            A.CallTo(() => locationNameService.GetLocationNameAsync(A<double>._, A<double>._)).Returns(new Result<LocationName>(locationName));

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.GetLatestLocationAsync(vehicle.VehicleId).Result;

            Assert.True(result.IsError);
            Assert.Equal(result.ErrorMessage, Message.VEHICLE_LOCATION_NOT_RECORDED);
        }

        [Fact]
        public void GetLatestLocationAsync_VehicleNotExisted()
        {
            var vehicle = new Vehicle
            {
                VehicleId = "v1",
                DeviceId = "d1",
                LastLongtitude = null,
                LastLatitude = null,
            };

            var locationName = new LocationName
            {
                Address = "Bangkok, Thailand",
                Latitude = 1,
                Longtitude = 1,
            };

            A.CallTo(() => vehicleRepository.GetVehicleById(A<string>._)).Returns(null);
            A.CallTo(() => locationNameService.GetLocationNameAsync(A<double>._, A<double>._)).Returns(new Result<LocationName>(locationName));

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.GetLatestLocationAsync(vehicle.VehicleId).Result;

            Assert.True(result.IsError);
            Assert.Equal(result.ErrorMessage, Message.VEHICLE_NOT_EXISTED);
        }

        [Fact]
        public void GetLocations_HasData()
        {
            var locationRecords = new List<Buckets.Models.LocationRecord>
            {
                new Buckets.Models.LocationRecord
                {
                    Latitude = 1,
                    Longtitude = 1,
                    Time = DateTime.Now.AddMinutes(1).Ticks,
                    VehicleId = "v1",
                },
                new Buckets.Models.LocationRecord
                {
                    Latitude = 2,
                    Longtitude = 2,
                    Time = DateTime.Now.AddMinutes(2).Ticks,
                    VehicleId = "v1",
                },
            };

            A.CallTo(() => vehicleRepository.GetLocations(A<string>._,A<DateTime>._, A<DateTime>._)).Returns(locationRecords);

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.GetLocations("v1",DateTime.Now,DateTime.Now.AddHours(1));

            Assert.False(result.IsError);
            Assert.Equal(result.Data.Count, locationRecords.Count);
        }

        [Fact]
        public void GetLocations_NoData()
        {
            var locationRecords = new List<Buckets.Models.LocationRecord>
            {
            };

            A.CallTo(() => vehicleRepository.GetLocations(A<string>._, A<DateTime>._, A<DateTime>._)).Returns(locationRecords);

            var vehicleService = new VehicleService(vehicleRepository, locationNameService);
            var result = vehicleService.GetLocations("v1", DateTime.Now, DateTime.Now.AddHours(1));

            Assert.True(result.IsError);
            Assert.Equal(result.ErrorMessage, Message.NO_LOCATION_RECORDED);
        }
    }
}
