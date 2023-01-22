using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

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
