using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Models
{
    public class ClientSongNote : IClientSongNote
    {
        public int SongNoteId { get; set; }
        public string CategoryName { get; set; }
        public string Content { get; set; }
        public int PlanSongId { get; set; }
    }
}
