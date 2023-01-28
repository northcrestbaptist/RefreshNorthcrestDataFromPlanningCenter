using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.App_BusinessLogic.Interfaces
{
    public interface ISongProcessor
    {
        Task<IList<IClientGeneralSong>> GetGeneralSongsAsync();
        Task<IList<IClientPlan_ForSongs>> GetPlansWithoutSongsAsync();
        Task<IClientPlan_ForSongs> GetPlanWithSongsAsync(int plan_ForSongsId);
        Task<IList<IClientPlan_ForSongs>> GetPlansWithSongsAsync();
        Task<byte[]> GetPdfAsync(int fileId);

        Task<byte[]> GetMp3Async(string filePath);
    }
}
