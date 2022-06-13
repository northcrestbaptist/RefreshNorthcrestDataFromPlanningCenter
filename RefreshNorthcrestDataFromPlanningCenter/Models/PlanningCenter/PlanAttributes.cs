using System;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter
{
    public class PlanAttributes
    {
        public bool multi_day { get; set; }
        public int items_count { get; set; }
        public string series_title { get; set; }
        // This is the effective date/time of the service.
        public DateTime sort_date { get; set; }
    }
}
