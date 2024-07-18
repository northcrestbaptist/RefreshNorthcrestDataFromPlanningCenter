using System;
using System.Collections.Generic;

namespace Northcrest.Domain.PlanningCenter
{
    public class Plan_ForSongs
    {
        public Plan_ForSongs()
        {
            PlanSongs = new List<PlanSong>();
        }

        public int Plan_ForSongsId { get; set; }
        public int PlanId { get; set; }
        public string Type { get; set; }
        public DateTime PlanDateTime { get; set; }
        public List<PlanSong> PlanSongs { get; set; }
    }
}
