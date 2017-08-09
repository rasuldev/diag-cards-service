using System;
using Newtonsoft.Json;

namespace EaisApi.Models
{
    public class VehicleRunningInfo
    {
        [JsonProperty("PROBEG")]
        public int Running { get; set; }
        [JsonProperty("DATA")]
        public DateTime Date { get; set; }
    }
}