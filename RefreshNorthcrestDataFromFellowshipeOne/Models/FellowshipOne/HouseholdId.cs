using Newtonsoft.Json;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class HouseholdId
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        [JsonProperty("@uri")]
        public string? uri { get; set; }
    }
}
