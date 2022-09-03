using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientAttachmentFactory
    {
        IClientAttachment CreateEmptyClientAttachment();
    }
}
