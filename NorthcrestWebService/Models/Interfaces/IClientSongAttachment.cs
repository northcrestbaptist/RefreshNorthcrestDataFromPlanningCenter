namespace NorthcrestWebService.Models.Interfaces
{
    public interface IClientSongAttachment
    {
        int SongAttachmentId { get; set; }
        string IconName { get; set; }
        string FilePath { get; set; }
        string FileName { get; set; }
        string FileType { get; set; }
        string ContentType { get; set; }
        int FileSize { get; set; }
        string Url { get; set; }
        int PlanSongId { get; set; }
    }
}
