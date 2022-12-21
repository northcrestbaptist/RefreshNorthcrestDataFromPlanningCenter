using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Domain
{
    public class SongAttachment
    {
        public int SongAttachmentId { get; set; }
        public byte[] File { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string ContentType { get; set; }

        public bool Downloadable { get; set; }
        public int FileSize { get; set; }
        public string Url { get; set; }
        public bool HasPreview { get; set; }
        public PlanSong PlanSong { get; set; }
        public int PlanSongId { get; set; }
    }
}
