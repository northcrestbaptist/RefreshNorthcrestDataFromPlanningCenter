using RefreshNorthcrestDataFromPlanningCenter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models
{
    public class Plans 
    {
        public Plan[] data { get; set; }
        public PlansMeta meta { get; set; }
    }
}
