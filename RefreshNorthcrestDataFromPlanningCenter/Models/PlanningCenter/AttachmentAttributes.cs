using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter
{
    public  class AttachmentAttributes
    {
        public bool allow_mp3_download { get; set; }
        public string content_type { get; set; }
        public string display_name { get; set; }

        public bool downloadable { get; set; }
        public int file_size { get; set; }
        
        public string filename { get; set; }
        public string filetype { get; set; }
        public bool has_preview { get; set; }
        public string thumbnail_url { get; set; }
        public string url { get; set; }
    }
}
