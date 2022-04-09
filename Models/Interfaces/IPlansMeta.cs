using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.Interfaces
{
    public interface IPlansMeta
    {
        int total_count { get; set; }
        int count { get; set; }
    }
}
