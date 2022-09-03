using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter
{
    public  class AttachmentAttributes
    {
        public string content_type { get; set; }

        public bool downloadable { get; set; }
        public int file_size { get; set; }
        
        public string filename { get; set; }
        public string url { get; set; }
        public string filetype { get; set; }
        public string thumbnail_url { get; set; }
        public bool has_preview { get; set; }

    }
}
