using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class PersonStatus
    {
        public string? name { get; set; }
        public string? comment { get; set; }
        public DateTime? date { get; set; }
        public PersonSubstatus? subStatus { get; set; }
    }
}
