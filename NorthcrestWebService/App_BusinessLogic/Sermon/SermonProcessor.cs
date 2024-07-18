using Microsoft.EntityFrameworkCore;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Data;
using NorthcrestWebService.Models.Interfaces;
using NorthcrestWebService.Common.FactoryInterfaces;
using Northcrest.Domain.PlanningCenter;

namespace NorthcrestWebService.App_BusinessLogic.ManifestUnit
{
    public class SermonProcessor : ISermonProcessor
    {
        private readonly IClientSermonFactory _clientSermonFactory;
        private readonly IClientAttachmentFactory _clientAttachmentFactory;

        public SermonProcessor(IClientSermonFactory clientSermonFactory, IClientAttachmentFactory clientAttachmentFactory) 
        {
            _clientSermonFactory = clientSermonFactory;
            _clientAttachmentFactory = clientAttachmentFactory;
        }

        public async Task<IList<IClientSermon>> GetSermonsAsync()
        {
            using var context = new NorthcrestDbContext();
            IList<Sermon> sermons = await context.Sermons.Include(a => a.Attachments).OrderByDescending(sermon => sermon.SermonDateTime).ToListAsync();
            IList<IClientSermon> clientSermons = await getClientSermonListFromSermonList(sermons);
            return clientSermons;
        }

        public async Task<byte[]> GetFileAsync(int fileId)
        {
            using var context = new NorthcrestDbContext();
            Attachment attachment = await context.Attachments.Where((x => x.AttachmentId == fileId)).FirstAsync();
            return attachment.File;
        }

        private async Task<IList<IClientSermon>> getClientSermonListFromSermonList(IList<Sermon> sermons)
        {
            IList <IClientSermon> clientSermonList = _clientSermonFactory.CreateClientSermonList();
            await Task.Run(() => { 
                foreach(Sermon sermon in sermons)
                {
                    IClientSermon clientSermon = _clientSermonFactory.CreateClientSermonFromSermon(sermon);
                    clientSermonList.Add(clientSermon);
                }
            });
            return clientSermonList;
        }
    }
}