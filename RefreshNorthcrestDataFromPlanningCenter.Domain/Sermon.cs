using System;

namespace RefreshNorthcrestDataFromPlanningCenter.Domain
{
    public class Sermon
    {
        public int Id { get; set; }
        public int PlanID { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SermonDateTime { get; set; }
        public string Speaker { get; set; }
    }
}
