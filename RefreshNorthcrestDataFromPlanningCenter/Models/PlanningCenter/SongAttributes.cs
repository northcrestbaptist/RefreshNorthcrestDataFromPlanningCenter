using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter
{
    public class SongAttributes
    {
        public string author { get; set; }
        //public int ccli_number { get; set; }
        public string copyright { get; set; }
        public DateTime last_scheduled_at { get; set; }
        public string title { get; set; }
        public string themes { get; set; }
    }
}
