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
    }
}
