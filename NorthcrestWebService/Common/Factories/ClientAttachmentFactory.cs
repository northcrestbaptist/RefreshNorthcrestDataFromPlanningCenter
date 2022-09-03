using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.Factories
{
    public class ClientAttachmentFactory : IClientAttachmentFactory
    {
        public IClientAttachment CreateEmptyClientAttachment()
        {
            return new ClientAttachment();
        }

    }
}
