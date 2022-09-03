using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.App_BusinessLogic.Interfaces
{
    public interface ISermonProcessor
    {
        Task<IList<IClientSermon>> GetSermonsAsync();
        Task<byte[]> GetFileAsync(int fileId);
    }
}