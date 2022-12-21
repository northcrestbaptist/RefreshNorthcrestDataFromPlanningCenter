using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.Local
{
    public class LocalAppConfiguration
    {
        public bool RefreshGeneralSongData { get; set; }
        public bool RefreshPlanSongData { get; set; }
        public bool RefreshSermonData { get; set; }
        public int NumberOfDaysToRefreshGeneralSongData { get; set; }
        public int NumberOfDaysToRefreshPlanSongData { get; set; } = 1;
        public int NumberOfDaysToRefreshSermonData { get; set; }
        public int NumberOfDaysToRefreshFutureData { get; set; }
    }
}
