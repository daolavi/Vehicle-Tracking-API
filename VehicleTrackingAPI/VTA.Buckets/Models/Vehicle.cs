namespace VTA.Buckets.Models
{
    public class Vehicle
    {
        public Vehicle()
        {
        }

        public string Type => typeof(Vehicle).Name;

        public string VehicleId { get; set; }

        public string DeviceId { get; set; }

        public double? LastLongtitude { get; set; }

        public double? LastLatitude { get; set; }

        //public double Fuel { get; set; } 

        //public double Speed { get; set; }
    }
}
