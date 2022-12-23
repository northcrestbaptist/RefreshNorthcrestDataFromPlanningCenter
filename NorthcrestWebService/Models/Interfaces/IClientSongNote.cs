using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Models.Interfaces
{
    public interface IClientSongNote
    {
        int SongNoteId { get; set; }
        string CategoryName { get; set; }
        string Content { get; set; }
        int PlanSongId { get; set; }
    }
}
