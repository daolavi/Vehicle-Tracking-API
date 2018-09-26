namespace VTA.Buckets.Models
{
    public class Vehicle
    {
        public Vehicle()
        {
        }

        public string Type => typeof(Vehicle).Name;

        // Each Vehicle has a vehicleId
        public string VehicleId { get; set; }

        // A device emboarded in a vehicle has a DeviceId. 
        // In case replacing the device for any reason, such as: damaged, ... , the new device has another DeviceId, but VehicleId doesn't change.
        public string DeviceId { get; set; }

        // For recording the latest longtitude
        public double? LastLongtitude { get; set; }

        // For recording the latest latitude
        public double? LastLatitude { get; set; }

        // In case adding Fuel property for data model
        //public double Fuel { get; set; } 

        // In case adding Speed property for data model
        //public double Speed { get; set; }
    }
}
