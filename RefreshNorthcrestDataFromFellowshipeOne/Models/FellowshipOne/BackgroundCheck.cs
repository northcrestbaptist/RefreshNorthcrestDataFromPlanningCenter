using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class BackgroundCheck
    {
        public int? trackingNumber { get; set; }
        public string? requestDate { get; set; }
        public BackgroundCheckStatus? backgroundCheckStatus { get; set; }
    }
}
