using VTA.Models.Request;
using VTA.Models.Response;

namespace VTA.Services.VehicleService
{
    public interface IVehicleService
    {
        Result<bool> Register(VehicleRegister registerVehicle);

        Result<bool> RecordLocation(LocationRecord recordPosition);

        bool IsRegistered(string vehicleId, string deviceId);

        bool IsPaired(string vehicleId, string deviceId);
    }
}
