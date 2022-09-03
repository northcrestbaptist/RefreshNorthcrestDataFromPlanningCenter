using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Models.Interfaces
{
    public interface IClientAttachment
    {
        int AttachmentId { get; set; }
        string IconName { get; set; }
        string FileName { get; set; }
        string FileType { get; set; }
        string ContentType { get; set; }
        int FileSize { get; set; }
        string Url { get; set; }
        int SermonId { get; set; }
    }
}
