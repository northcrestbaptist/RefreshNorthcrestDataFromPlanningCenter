using RefreshNorthcrestDataFromPlanningCenter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models
{
    public class PlansMeta : IPlansMeta
    {
        public int total_count { get; set; }
        public int count { get; set; }
    }
}
