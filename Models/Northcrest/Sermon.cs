using RefreshNorthcrestDataFromPlanningCenter.Common;
using System;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.Northcrest
{
    public class Sermon
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SermonDateTime { get; set; }
        public string Speaker { get; set; }
        public ServiceType Type { get; set; }
    }
}
