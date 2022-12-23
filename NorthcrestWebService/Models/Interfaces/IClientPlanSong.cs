using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Models.Interfaces
{
    public interface IClientPlanSong
    {
        int PlanSongId { get; set; }
        int SongId { get; set; }
        int ArrangementId { get; set; }
        string SongName { get; set; }
        string ArrangementName { get; set; }
        string Author { get; set; }
        string Copyright { get; set; }
        string Length { get; set; }
        string KeyName { get; set; }
        string Description { get; set; }
        int Sequence { get; set; }
        string Notes { get; set; }
        List<IClientSongNote> SongNotes { get; set; }
        List<IClientSongAttachment> SongAttachments { get; set; }
        int Plan_ForSongsId { get; set; }
    }
}
