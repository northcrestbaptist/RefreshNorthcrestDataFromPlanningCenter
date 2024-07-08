using Newtonsoft.Json;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class RequirementStatus
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        public string? name { get; set; }
    }
}
