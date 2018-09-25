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

        //public decimal Fuel { get; set; } 

        //public decimal Speed { get; set; }
    }
}
