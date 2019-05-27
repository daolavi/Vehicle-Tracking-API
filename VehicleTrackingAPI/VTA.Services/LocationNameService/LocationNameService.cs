using Geocoding;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTA.Constants;
using VTA.Models.Response;

namespace VTA.Services.LocationNameService
{
    public class LocationNameService : ILocationNameService
    {
        private readonly IDistributedCache cache;

        private readonly IGeocoder geoCoder;

        public LocationNameService(IDistributedCache cache, IGeocoder geoCoder)
        {
            this.cache = cache;
            this.geoCoder = geoCoder;
        }

        public async Task<Result<LocationName>> GetLocationNameAsync(double latitude, double longtitude)
        {
            var latlng = $"{latitude},{longtitude}";
            var address = cache.GetString(latlng);
            if (string.IsNullOrEmpty(address))
            {
                IEnumerable<Address> addresses = await geoCoder.ReverseGeocodeAsync(latitude, longtitude);
                var formattedAddress = addresses.Count() == 0 ? Message.ADDRESS_NOT_FOUND : addresses.FirstOrDefault().FormattedAddress;
                await cache.SetStringAsync(latlng, formattedAddress);
                return new Result<LocationName>(new LocationName { Latitude = latitude, Longtitude = longtitude, Address = formattedAddress });
            }
            else
            {
                return new Result<LocationName>(new LocationName { Latitude = latitude, Longtitude = longtitude, Address = address});
            }
        }
    }
}
