using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Domain
{
    public class PlanSong
    {
        public PlanSong()
        {
            SongNotes = new List<SongNote>();
            SongAttachments = new List<SongAttachment>();
        }
        public int PlanSongId { get; set; }
        public int SongId { get; set; }
        public int ArrangementId { get; set; }
        public string SongName { get; set; }
        public string ArrangementName { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public int Length { get; set; }
        public string KeyName { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public string Notes { get; set; }
        public List<SongNote> SongNotes { get; set; }
        public List<SongAttachment> SongAttachments { get; set; }
        public Plan_ForSongs Plan_ForSongs { get; set; }
        public int Plan_ForSongsId { get; set; }
    }
}
