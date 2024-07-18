using Microsoft.EntityFrameworkCore;
using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Data;

namespace NorthcrestWebService.App_BusinessLogic.Song
{
    public class SongProcessor : ISongProcessor
    {
        private readonly IClientSongFactory _clientSongFactory;
        private readonly IClientAttachmentFactory _clientAttachmentFactory;
        private readonly IClientPlan_ForSongsFactory _clientPlan_ForSongsFactory;
        private readonly ILogger<SongProcessor> _logger;

        public SongProcessor(
            IClientSongFactory clientSongFactory, 
            IClientAttachmentFactory clientAttachmentFactory,
            IClientPlan_ForSongsFactory clientPlan_ForSongsFactory,
            ILogger<SongProcessor> logger)
        {
            _clientSongFactory = clientSongFactory;
            _clientAttachmentFactory = clientAttachmentFactory;
            _clientPlan_ForSongsFactory = clientPlan_ForSongsFactory;
            _logger = logger;
        }

        public async Task<IList<IClientGeneralSong>> GetGeneralSongsAsync()
        {
            using var context = new NorthcrestDbContext();
            IList<GeneralSong> generalSongs = await context.GeneralSongs
                .OrderBy(song => song.SongName)
                .ThenBy(song => song.ArrangementName)
                .ToListAsync();
            IList<IClientGeneralSong> clientGeneralSongs = await getClientGeneralSongListFromGeneralSongListAsync(generalSongs);
            return clientGeneralSongs;
        }

        public async Task<IList<IClientPlan_ForSongs>> GetPlansWithoutSongsAsync()
        {
            using var context = new NorthcrestDbContext();
            IList<Plan_ForSongs> plan_ForSongsList = await context.Plan_ForSongs
                .Where(s => s.PlanDateTime.Date >= DateTime.Now.Date)
                .OrderBy(plan => plan.PlanDateTime)
                .ToListAsync();
            IList<IClientPlan_ForSongs> clientPlan_ForSongs = await getClientPlan_ForSongsListFromPlan_ForSongsListAsync(plan_ForSongsList);
            return clientPlan_ForSongs;
        }

        public async Task<IClientPlan_ForSongs> GetPlanWithSongsAsync(int planId)
        {
            using var context = new NorthcrestDbContext();
            Plan_ForSongs plan_ForSongs = await context.Plan_ForSongs
                .Where(p => p.PlanId == planId)
                .AsSplitQuery()
                .Include(s => s.PlanSongs).ThenInclude(s => s.SongNotes)
                .AsSplitQuery()
                .Include(s => s.PlanSongs).ThenInclude(s => s.SongAttachments)
                .FirstAsync();
            IClientPlan_ForSongs clientPlan_ForSongs = 
                _clientPlan_ForSongsFactory.CreateClientPlan_ForSongsFromPlan_ForSongs(plan_ForSongs);
            return clientPlan_ForSongs;
        }

        public async Task<IList<IClientPlan_ForSongs>> GetPlansWithSongsAsync()
        {
            using var context = new NorthcrestDbContext();
            IList<Plan_ForSongs> plan_ForSongsList = await context.Plan_ForSongs
                .Where(s => s.PlanDateTime.Date >= DateTime.Now.Date)
                .Include(s => s.PlanSongs).ThenInclude(s => s.SongNotes)
                .Include(s => s.PlanSongs).ThenInclude(s => s.SongAttachments)
                .OrderBy(plan => plan.PlanDateTime)
                .ToListAsync();
            IList<IClientPlan_ForSongs> clientPlan_ForSongs = await getClientPlan_ForSongsListFromPlan_ForSongsListAsync(plan_ForSongsList);
            return clientPlan_ForSongs;
        }

        public async Task<byte[]>GetPdfAsync(int fileId)
        {
            using var context = new NorthcrestDbContext();
            SongAttachment attachment = await context.SongAttachments.Where((x => x.SongAttachmentId == fileId)).FirstAsync();
            return attachment.File;
        }

        public async Task<byte[]>GetMp3Async(string filePath)
        {
            var bytes = await File.ReadAllBytesAsync(filePath);
            return bytes;
        }

        private async Task<IList<IClientGeneralSong>> getClientGeneralSongListFromGeneralSongListAsync(IList<GeneralSong> generalSongs)
        {
            IList<IClientGeneralSong> clientGeneralSongList = _clientSongFactory.CreateClientGeneralSongList();
            await Task.Run(() => {
                foreach (GeneralSong song in generalSongs)
                {
                    IClientGeneralSong ClientGeneralSong = _clientSongFactory.CreateClientGeneralSongFromGeneralSong(song);
                    clientGeneralSongList.Add(ClientGeneralSong);
                }
            });
            return clientGeneralSongList;
        }

        private async Task<IList<IClientPlan_ForSongs>> getClientPlan_ForSongsListFromPlan_ForSongsListAsync(IList<Plan_ForSongs> plan_ForSongsList)
        {
            IList<IClientPlan_ForSongs> clientPlan_ForSongsList = _clientPlan_ForSongsFactory.CreateClientPlan_ForSongsList();
            await Task.Run(() =>
            {
                foreach (Plan_ForSongs plan in plan_ForSongsList)
                {
                    IClientPlan_ForSongs clientPlan_ForSongs = _clientPlan_ForSongsFactory.CreateClientPlan_ForSongsFromPlan_ForSongs(plan);
                    clientPlan_ForSongsList.Add(clientPlan_ForSongs);
                }
            });
            return clientPlan_ForSongsList;
        }
    }
}
