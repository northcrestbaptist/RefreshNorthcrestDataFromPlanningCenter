using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Models
{
    public class ClientAttachment : IClientAttachment
    {
        public int AttachmentId { get; set; }
        public string IconName { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string ContentType { get; set; }
        public int FileSize { get; set; }
        public string Url { get; set; }
        public int SermonId { get; set; }
    }
}
