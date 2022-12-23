namespace NorthcrestWebService.Models.Interfaces
{
    public interface IClientGeneralSong
    {
        int Id { get; set; }
        int SongId { get; set; }
        int ArrangementId { get; set; }
        string SongName { get; set; }
        string ArrangementName { get; set; }
        string Author { get; set; }
        string Copyright { get; set; }
        string Length { get; set; }
        string Themes { get; set; }
        DateTime LastScheduledDateTime { get; set; }
    }
}
