using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.Interfaces
{
    public interface IPlans
    {
        IPlan[] data { get; set; }
        IPlansMeta meta { get; set; }
    }
}
