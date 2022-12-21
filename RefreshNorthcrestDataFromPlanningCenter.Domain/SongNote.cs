using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Domain
{
    public class SongNote
    {
        public int SongNoteId { get; set; }
        public string CategoryName { get; set; }
        public string Content { get; set; }
        public PlanSong PlanSong { get; set; }
        public int PlanSongId { get; set; }
    }
}
