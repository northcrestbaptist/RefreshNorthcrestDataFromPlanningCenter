using Newtonsoft.Json;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class Requirement
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        public string? name { get; set; }
    }
}
