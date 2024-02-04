using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientAttachmentFactory
    {
        IClientAttachment CreateEmptyClientAttachment();
        IClientSongAttachment CreateEmptyClientSongAttachment();

        IClientAttachment CreateClientAttachment(Attachment attachment);
        IClientSongAttachment CreateClientSongAttachment(SongAttachment songAttachment);
    }
}
