using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class PersonAttribute
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        [JsonProperty("@uri")]
        public string? uri { get; set; }
        public PersonAttributeGroup? attributeGroup { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? comment { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastUpdateddDate { get; set; }
    }
}
