using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models.Interfaces;
using NorthcrestWebService.Models;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Common.Factories
{
    public class ClientSongFactory : IClientSongFactory
    {
        private readonly IClientAttachmentFactory _clientAttachmentFactory;
        private readonly IClientNoteFactory _clientNoteFactory;

        public ClientSongFactory(IClientAttachmentFactory clientAttachmentFactory,
            IClientNoteFactory clientNoteFactory)
        {
            _clientAttachmentFactory = clientAttachmentFactory;
            _clientNoteFactory = clientNoteFactory;
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
            foreach (SongAttachment attachment in clientPlanSong.SongAttachments)
            {
                IClientSongAttachment clientAttachment = _clientAttachmentFactory.CreateEmptyClientSongAttachment();
                clientAttachment.SongAttachmentId = attachment.SongAttachmentId;
                clientAttachment.Url = attachment.Url;
                clientAttachment.ContentType = attachment.ContentType;
                clientAttachment.FileName = attachment.FileName;
                clientAttachment.FileSize = attachment.FileSize;
                clientAttachment.FileType = attachment.FileType;
                clientAttachment.PlanSongId = attachment.PlanSongId;
                switch (attachment.FileType)
                {
                    case "pdf":
                        clientAttachment.IconName = "newspaper";
                        break;
                    case "audio":
                        clientAttachment.IconName = "musical-notes";
                        break;
                    case "file":
                        clientAttachment.IconName = "document-text";
                        break;
                    case "video":
                        clientAttachment.IconName = "videocam";
                        break;
                    default:
                        clientAttachment.IconName = "help";
                        break;
                }
                clientPlanSong.SongAttachments.Add(clientAttachment);
            }

            foreach (SongNote songNote in clientPlanSong.SongNotes)
            {
                IClientSongNote clientNote = _clientNoteFactory.CreateEmptyClientSongNote();
                clientNote.SongNoteId = songNote.SongNoteId;
                clientNote.CategoryName = songNote.CategoryName;
                clientNote.Content = songNote.Content;
                clientNote.PlanSongId = songNote.PlanSongId;
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
