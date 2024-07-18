using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class PersonCommunication
    {
        public HouseholdId? household { get; set; }
        public CommunicationType? communicationType { get; set; }
        public string? communicationGeneralType { get; set; }
        public string? communicationValue { get; set; }
        public string? searchCommunicationValue { get; set; }
        public bool? preferred { get; set; }
        public string? communicationComment { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastUpdatedDate { get; set; }

    }
}
