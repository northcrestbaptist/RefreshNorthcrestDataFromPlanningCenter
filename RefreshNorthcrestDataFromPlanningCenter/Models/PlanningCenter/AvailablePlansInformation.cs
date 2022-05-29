using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter
{
    public class AvailablePlansInformation
    {
        public ServiceType Type { get; set; }
        public int TotalRecordsAvailable { get; set; }
        public int RemainingRecords { get; set; }
        public int OffSet { get; set; } = 0;
        public Plans CurrentRetrievedPlans { get; set; }
        public Items CurrentRetrievedItems { get; set; }
        public ItemNotes CurrentRetrievedItemNotes { get; set; }

    }
}
