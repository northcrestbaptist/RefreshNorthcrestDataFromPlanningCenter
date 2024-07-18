using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.Factories
{
    public class ClientNoteFactory : IClientNoteFactory
    {
        public IClientSongNote CreateEmptyClientSongNote()
        {
            return new ClientSongNote();
        }

        public IClientSongNote CreateClientSongNote(SongNote songNote)
        {
            IClientSongNote clientSongNote = CreateEmptyClientSongNote();
            clientSongNote.SongNoteId = songNote.SongNoteId;
            clientSongNote.CategoryName = songNote.CategoryName;
            clientSongNote.Content = songNote.Content;
            clientSongNote.PlanSongId = songNote.PlanSongId;

            return clientSongNote;
        }
    }
}
