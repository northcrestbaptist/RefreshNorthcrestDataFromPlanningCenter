using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientNoteFactory
    {
        IClientSongNote CreateEmptyClientSongNote();
        IClientSongNote CreateClientSongNote(SongNote songNote);
    }
}
