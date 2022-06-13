using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter
{
    public class AvailablePlansInformation
    {
        public DateTime MostRecentSermonInNorthcrestDatabase { get; set; }
        public HttpClient Client { get; set; }
        public PlanningCenterConfiguration Configuration { get; set; }
        public string Url { get; set; }
        public int NumberOfRequests { get; set; }
        public int PlanIndex { get; set; }
        public string Type { get; set; }
        public int TotalRecordsAvailable { get; set; }
        public int RemainingRecords { get; set; }
        public int OffSet { get; set; } = 0;
        public Plans CurrentRetrievedPlans { get; set; } 
        public Items CurrentRetrievedItems { get; set; }
        public ItemNotes CurrentRetrievedItemNotes { get; set; }
        public IList<Sermon> Sermons { get; set; } = new List<Sermon>();    

    }
}
