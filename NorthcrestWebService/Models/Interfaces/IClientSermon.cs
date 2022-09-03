namespace NorthcrestWebService.Models.Interfaces
{
    public interface IClientSermon
    {
        int SermonId { get; set; }
        int PlanId { get; set; }
        string Type { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime SermonDateTime { get; set; }
        string Speaker { get; set; }
        List<IClientAttachment> Attachments { get; set; }
    }
}
