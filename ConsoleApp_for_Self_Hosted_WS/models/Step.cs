using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp_for_Self_Hosted_WS.models
{
    public class Step
    {
        [JsonPropertyName("instruction")] public string Instruction { get; set; }
        [JsonPropertyName("distance")] public double Distance { get; set; }
        [JsonPropertyName("duration")] public double Duration { get; set; }
    }

    public class OSR
    {
        [JsonPropertyName("routes")] public List<Route> Routes { get; set; }
    }

    public class Route
    {
        [JsonPropertyName("segments")] public List<Segment> Segments { get; set; }
    }

    public class Segment
    {
        [JsonPropertyName("steps")] public List<Step> Steps { get; set; }
    }
}

