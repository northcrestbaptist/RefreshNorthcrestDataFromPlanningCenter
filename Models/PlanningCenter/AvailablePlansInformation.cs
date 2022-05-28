using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter
{
    public class AvailablePlansInformation
    {
        public int TotalRecordsAvailable { get; set; }
        public int RemainingRecords { get; set; }
        public int OffSet { get; set; } = 0;
        public Plans CurrentRetrievedPlans { get; set; }
        public Items CurrentRetrievedItems { get; set; }
        public ItemNotes CurrentRetrievedItemNotes { get; set; }

    }
}
