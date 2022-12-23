using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.App_BusinessLogic.Interfaces
{
    public interface ISongProcessor
    {
        Task<IList<IClientGeneralSong>> GetGeneralSongsAsync();
        Task<IList<IClientPlan_ForSongs>> GetPlansWithSongsAsync();
        Task<byte[]> GetPdfAsync(int fileId);
    }
}
