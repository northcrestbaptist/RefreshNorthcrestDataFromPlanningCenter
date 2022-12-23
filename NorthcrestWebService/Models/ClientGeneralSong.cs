using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Models
{
    public class ClientGeneralSong : IClientGeneralSong
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public int ArrangementId { get; set; }
        public string SongName { get; set; }
        public string ArrangementName { get; set; }
        public string Author { get; set; }
        public string Copyright { get; set; }
        public string Length { get; set; }
        public string Themes { get; set; }
        public DateTime LastScheduledDateTime { get; set; }
    }
}
