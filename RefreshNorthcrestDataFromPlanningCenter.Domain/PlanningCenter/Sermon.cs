using System;
using System.Collections.Generic;

namespace Northcrest.Domain.PlanningCenter
{
    public class Sermon
    {
        public Sermon()
        {
            Attachments = new List<Attachment>();
        }

        public int SermonId { get; set; }
        public int PlanId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SermonDateTime { get; set; }
        public string Speaker { get; set; }
        public List<Attachment> Attachments { get; set; }
    }
}
