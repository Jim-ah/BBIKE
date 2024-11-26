using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp_for_Self_Hosted_WS.models
{
    public class Station
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("position")] public Coordinates Coordinates { get; set; }
        [JsonPropertyName("available_bikes")] public int AvailableBikes { get; set; }
    }
}
