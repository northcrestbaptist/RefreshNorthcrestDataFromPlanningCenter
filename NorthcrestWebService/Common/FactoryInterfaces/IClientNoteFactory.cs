using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientNoteFactory
    {
        IClientSongNote CreateEmptyClientSongNote();
        IClientSongNote CreateClientSongNote(SongNote songNote);
    }
}
