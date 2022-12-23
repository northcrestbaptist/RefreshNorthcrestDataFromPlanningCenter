using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientNoteFactory
    {
        IClientSongNote CreateEmptyClientSongNote();
    }
}
