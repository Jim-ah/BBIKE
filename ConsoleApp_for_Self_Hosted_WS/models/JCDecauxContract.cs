using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp_for_Self_Hosted_WS.models
{
    public class JCDecauxContract
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("country_code")] public string CountryCode { get; set; }

        [JsonPropertyName("cities")] public List<string> Cities { get; set; }
    }
}
