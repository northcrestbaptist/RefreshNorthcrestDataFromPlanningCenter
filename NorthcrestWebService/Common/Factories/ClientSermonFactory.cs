using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System.Text;

namespace NorthcrestWebService.Common.Factories
{
    public class ClientSermonFactory : IClientSermonFactory
    {
        private readonly IClientAttachmentFactory _clientAttachmentFactory;

        public ClientSermonFactory(IClientAttachmentFactory clientAttachmentFactory)
        {
            _clientAttachmentFactory = clientAttachmentFactory;
        }

        public IClientSermon CreateEmptyClientSermon()
        {
            return new ClientSermon();
        }

        public IClientSermon CreateClientSermonFromSermon(Sermon sermon)
        {
            IClientSermon clientSermon = CreateEmptyClientSermon();
            clientSermon.SermonId = sermon.SermonId;
            clientSermon.Title = sermon.Title;
            clientSermon.SermonDateTime = sermon.SermonDateTime;
            clientSermon.Description = sermon.Description;
            clientSermon.Speaker = sermon.Speaker;
            clientSermon.PlanId = sermon.PlanId;
            clientSermon.Type = sermon.Type;
            foreach(Attachment attachment in sermon.Attachments)
            {
                IClientAttachment clientAttachment = _clientAttachmentFactory.CreateEmptyClientAttachment();
                clientAttachment.AttachmentId = attachment.AttachmentId;
                clientAttachment.Url = attachment.Url;
                clientAttachment.ContentType = attachment.ContentType;
                clientAttachment.FileName = attachment.FileName;
                clientAttachment.FileSize = attachment.FileSize;
                clientAttachment.FileType = attachment.FileType;
                clientAttachment.SermonId = attachment.SermonId;
                switch(attachment.FileType)
                {
                    case "pdf":
                        clientAttachment.IconName = "file";
                        break;
                    case "video":
                        clientAttachment.IconName = "video";
                        break;
                    default:
                        clientAttachment.IconName = "question";
                        break;
                }
                clientSermon.Attachments.Add(clientAttachment);
            }
            return clientSermon;
        }

        public IList<IClientSermon> CreateClientSermonList()
        {
            return new List<IClientSermon>();
        }
    }
}
