using FakeItEasy;
using Geocoding;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using VTA.Constants;
using VTA.Services.LocationNameService;
using Xunit;

namespace VTA.Tests
{
    public class LocationNameServiceTest
    {
        private readonly IDistributedCache cache;

        private readonly IGeocoder geoCoder;

        public LocationNameServiceTest()
        {
            cache = A.Fake<IDistributedCache>();
            geoCoder = A.Fake<IGeocoder>();
        }

        [Fact]
        public async void GetLocationNameAsync_HasDataAsync()
        {
            var addresses = new List<ParsedAddress>
            {
                new ParsedAddress("Bangkok, ThaiLand", new Location(1,1), "Google Map Api"),
                new ParsedAddress("Pattaya, ThaiLand", new Location(1,1), "Google Map Api"),
            };

            A.CallTo(() => geoCoder.ReverseGeocodeAsync(A<double>._,A<double>._)).Returns(addresses);

            var locationNameService = new LocationNameService(cache,geoCoder);
            var result = await locationNameService.GetLocationNameAsync(1, 1);

            Assert.Equal(result.Data.Address, addresses[0].FormattedAddress);
        }

        [Fact]
        public async void GetLocationNameAsync_AddressNotFoundAsync()
        {
            var addresses = new List<ParsedAddress>
            {
            };

            A.CallTo(() => geoCoder.ReverseGeocodeAsync(A<double>._, A<double>._)).Returns(addresses);

            var locationNameService = new LocationNameService(cache, geoCoder);
            var result = await locationNameService.GetLocationNameAsync(1, 1);

            Assert.Equal(result.Data.Address, Message.ADDRESS_NOT_FOUND);
        }
    }
}
