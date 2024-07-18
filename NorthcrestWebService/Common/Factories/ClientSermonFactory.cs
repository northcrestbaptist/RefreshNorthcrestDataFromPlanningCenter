using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;

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
                IClientAttachment clientAttachment = _clientAttachmentFactory.CreateClientAttachment(attachment);
                
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
