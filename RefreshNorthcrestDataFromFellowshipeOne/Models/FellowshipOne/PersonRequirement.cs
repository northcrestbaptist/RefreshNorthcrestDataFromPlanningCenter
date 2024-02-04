using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? requirementDate { get; set; }
        public StaffPerson? staffPerson { get; set; }
        public BackgroundCheck? backgroundCheck { get; set; }
        public string? createdDate { get; set; }
        public string? lastUpdatedDate { get; set; }
    }
}
