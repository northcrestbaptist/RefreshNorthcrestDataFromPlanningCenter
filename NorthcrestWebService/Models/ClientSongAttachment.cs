using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Models
{
    public class ClientSongAttachment : IClientSongAttachment
    {
        public int SongAttachmentId { get; set; }
        public string IconName { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string ContentType { get; set; }
        public int FileSize { get; set; }
        public string Url { get; set; }
        public int PlanSongId { get; set; }
    }
}
