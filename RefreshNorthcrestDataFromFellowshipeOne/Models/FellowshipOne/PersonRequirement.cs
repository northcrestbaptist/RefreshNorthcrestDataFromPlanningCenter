using Newtonsoft.Json;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class PersonRequirement
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        [JsonProperty("@uri")]
        public string? uri { get; set; }
        [JsonProperty("@requirementDocumentURI")]
        public string? requirementDocumentUri { get; set; }
        public RequirementPerson? person { get; set; }
        public Requirement? requirement { get; set; }
        public RequirementStatus? requirementStatus { get; set; }
        public DateTime? requirementDate { get; set; }
        public StaffPerson? staffPerson { get; set; }
        public BackgroundCheck? backgroundCheck { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
    }
}
