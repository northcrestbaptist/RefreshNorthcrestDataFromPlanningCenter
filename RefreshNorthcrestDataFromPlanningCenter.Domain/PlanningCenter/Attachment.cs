using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northcrest.Domain.PlanningCenter
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string ContentType { get; set; }

        public bool Downloadable { get; set; }
        public int FileSize { get; set; }
        public string Url { get; set; }
        public bool HasPreview { get; set; }
        public Sermon Sermon { get; set; }
        public int SermonId { get; set; }
    }
}
