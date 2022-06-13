using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public  interface INorthcrestLocalData
    {
        DateTime GetLatestSermonDateTime();
    }
}
