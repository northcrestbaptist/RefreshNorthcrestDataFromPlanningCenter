using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Models
{
    public class ClientPlanSong : IClientPlanSong
    {
        public int PlanSongId { get; set; }
        public int SongId { get; set; }
        public int ArrangementId { get; set; }
        public string SongName { get; set; }
        public string ArrangementName { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public string Length { get; set; }
        public string KeyName { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public string Notes { get; set; }
        public List<IClientSongNote> SongNotes { get; set; } = new List<IClientSongNote>();
        public List<IClientSongAttachment> SongAttachments { get; set; } = new List<IClientSongAttachment>(); 
        public int Plan_ForSongsId { get; set; }
    }
}
