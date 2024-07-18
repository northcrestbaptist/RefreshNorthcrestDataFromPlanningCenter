using Northcrest.Domain.PlanningCenter;
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

        public IClientSongAttachment CreateEmptyClientSongAttachment()
        {
            return new ClientSongAttachment();
        }

        public IClientAttachment CreateClientAttachment(Attachment attachment)
        {
            IClientAttachment clientAttachment = CreateEmptyClientAttachment();
            clientAttachment.AttachmentId = attachment.AttachmentId;
            clientAttachment.Url = attachment.Url;
            clientAttachment.ContentType = attachment.ContentType;
            clientAttachment.FileName = attachment.FileName;
            clientAttachment.FileSize = attachment.FileSize;
            clientAttachment.FileType = attachment.FileType;
            clientAttachment.SermonId = attachment.SermonId;
            switch (attachment.FileType)
            {
                case "pdf":
                    clientAttachment.IconName = "newspaper";
                    break;
                case "video":
                    clientAttachment.IconName = "videocam";
                    break;
                default:
                    clientAttachment.IconName = "help";
                    break;
            }
            return clientAttachment;

        }

        public IClientSongAttachment CreateClientSongAttachment(SongAttachment songAttachment)
        {
            IClientSongAttachment clientSongAttachment = CreateEmptyClientSongAttachment();
            
            clientSongAttachment.SongAttachmentId = songAttachment.SongAttachmentId;
            clientSongAttachment.Url = songAttachment.Url;
            clientSongAttachment.ContentType = songAttachment.ContentType;
            clientSongAttachment.FileName = songAttachment.FileName;
            clientSongAttachment.FileSize = songAttachment.FileSize;
            clientSongAttachment.FileType = songAttachment.FileType;
            clientSongAttachment.FilePath = songAttachment.FilePath;
            clientSongAttachment.PlanSongId = songAttachment.PlanSongId;
            switch (songAttachment.FileType)
            {
                case "pdf":
                    clientSongAttachment.IconName = "newspaper";
                    break;
                case "audio":
                    clientSongAttachment.IconName = "musical-notes";
                    break;
                case "file":
                    clientSongAttachment.IconName = "document-text";
                    break;
                case "video":
                    clientSongAttachment.IconName = "videocam";
                    break;
                default:
                    clientSongAttachment.IconName = "help";
                    break;
            }
            return clientSongAttachment;
        }
    }
}
