using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class StaffPerson
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        [JsonProperty("@uri")]
        public string? uri { get; set; }
    }
}
