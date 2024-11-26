using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp_for_Self_Hosted_WS.models
{
    public class MLocation
    {
        public string Address { get; }
        public Coordinates Coordinates { get; }

        public MLocation(Coordinates coordinates)
        {
            Coordinates = coordinates;
        }
    }

    public class Coordinates
    {
        [JsonPropertyName("lat")] public float Latitude { get; set; }
        [JsonPropertyName("lng")] public float Longitude { get; set; }

        public Coordinates(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double GetDistanceTo(Coordinates other)
        {
            // Haversine formula to calculate distance between two points
            double R = 6371000;
            double lat1Rad = ToRadians(Latitude);
            double lat2Rad = ToRadians(other.Latitude);
            double deltaLat = ToRadians(other.Latitude - Latitude);
            double deltaLon = ToRadians(other.Longitude - Longitude);

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in meters
        }

        private double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
