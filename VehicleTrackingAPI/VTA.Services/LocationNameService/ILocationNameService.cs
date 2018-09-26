using System.Threading.Tasks;
using VTA.Models.Response;

namespace VTA.Services.LocationNameService
{
    public interface ILocationNameService
    {
        Task<Result<LocationName>> GetLocationNameAsync(double latitude, double longtitude);
    }
}
