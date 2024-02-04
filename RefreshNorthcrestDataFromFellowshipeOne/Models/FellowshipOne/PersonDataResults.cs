using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class PersonDataResults
    {
        [JsonProperty("@count")]
        public int? count { get; set; }
        [JsonProperty("@pageNumber")]
        public int? pageNumber { get; set; }
        [JsonProperty("@totalRecords")]
        public int totalRecords { get; set; }
        [JsonProperty("@additionalPages")]
        public int? additionalPages { get; set; }
        public required Person[] person { get; set; } 
    }
}
