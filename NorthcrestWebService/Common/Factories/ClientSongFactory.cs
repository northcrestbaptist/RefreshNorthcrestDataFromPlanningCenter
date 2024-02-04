using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models.Interfaces;
using NorthcrestWebService.Models;
using Northcrest.Domain.PlanningCenter;

namespace NorthcrestWebService.Common.Factories
{
    public class ClientSongFactory : IClientSongFactory
    {
        private readonly IClientAttachmentFactory _clientAttachmentFactory;
        private readonly IClientNoteFactory _clientNoteFactory;
        private readonly ILogger<ClientSongFactory> _logger;

        public ClientSongFactory(IClientAttachmentFactory clientAttachmentFactory,
            IClientNoteFactory clientNoteFactory,
            ILogger<ClientSongFactory> logger)
        {
            _clientAttachmentFactory = clientAttachmentFactory;
            _clientNoteFactory = clientNoteFactory;
            _logger = logger;
        }

        public IClientGeneralSong CreateEmptyClientGeneralSong()
        {
            return new ClientGeneralSong();
        }

        public IClientPlanSong CreateEmptyClientPlanSong()
        {
            return new ClientPlanSong();
        }

        public IClientGeneralSong CreateClientGeneralSongFromGeneralSong(GeneralSong generalSong)
        {
            IClientGeneralSong clientGeneralSong = CreateEmptyClientGeneralSong();
            clientGeneralSong.Id = generalSong.Id;
            clientGeneralSong.ArrangementId = generalSong.ArrangementId;
            clientGeneralSong.ArrangementName = generalSong.ArrangementName;
            clientGeneralSong.SongId = generalSong.SongId;
            clientGeneralSong.SongName = generalSong.SongName;
            clientGeneralSong.Themes = generalSong.Themes;
            clientGeneralSong.Author = generalSong.Author;
            clientGeneralSong.Copyright = generalSong.Copyright;
            clientGeneralSong.LastScheduledDateTime = generalSong.LastScheduledDateTime;
            TimeSpan time = TimeSpan.FromSeconds(generalSong.Length);
            clientGeneralSong.Length = time.ToString(@"mm\:ss");

            return clientGeneralSong;
        }

        public IClientPlanSong CreateClientPlanSongFromPlanSong(PlanSong planSong)
        {
            IClientPlanSong clientPlanSong = CreateEmptyClientPlanSong();
            clientPlanSong.Plan_ForSongsId = planSong.Plan_ForSongsId;
            clientPlanSong.PlanSongId = planSong.PlanSongId;
            clientPlanSong.ArrangementId = planSong.ArrangementId;
            clientPlanSong.ArrangementName = planSong.ArrangementName;
            clientPlanSong.SongId = planSong.SongId;
            clientPlanSong.SongName = planSong.SongName;
            clientPlanSong.Author = planSong.Author;
            clientPlanSong.Copyright = planSong.Copyright;
            clientPlanSong.Description = planSong.Description;
            clientPlanSong.KeyName = planSong.KeyName;
            clientPlanSong.Notes = planSong.Notes;
            clientPlanSong.Sequence = planSong.Sequence;
            TimeSpan time = TimeSpan.FromSeconds(planSong.Length);
            clientPlanSong.Length = time.ToString(@"mm\:ss");
           
            foreach (SongAttachment attachment in planSong.SongAttachments)
            {
                IClientSongAttachment clientAttachment = _clientAttachmentFactory.CreateClientSongAttachment(attachment);
                clientPlanSong.SongAttachments.Add(clientAttachment);
            }

            foreach (SongNote songNote in planSong.SongNotes)
            {
                IClientSongNote clientNote = _clientNoteFactory.CreateClientSongNote(songNote);
                clientPlanSong.SongNotes.Add(clientNote);
            }

            return clientPlanSong;
        }

        public IList<IClientGeneralSong> CreateClientGeneralSongList()
        {
            return new List<IClientGeneralSong>();
        }

        public IList<IClientPlanSong> CreateClientPlanSongList()
        {
            return new List<IClientPlanSong>();
        }
    }
}
