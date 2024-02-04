using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class RequirementStatus
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        public string? name { get; set; }
    }
}
