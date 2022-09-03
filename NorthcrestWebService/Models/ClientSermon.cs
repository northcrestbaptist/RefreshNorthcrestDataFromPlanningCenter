using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Models
{
    public class ClientSermon : IClientSermon
    {
        public int SermonId { get; set; }
        public int PlanId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SermonDateTime { get; set; }
        public string Speaker { get; set; }
        public List<IClientAttachment> Attachments { get; set; } = new List<IClientAttachment>();
    }
}
