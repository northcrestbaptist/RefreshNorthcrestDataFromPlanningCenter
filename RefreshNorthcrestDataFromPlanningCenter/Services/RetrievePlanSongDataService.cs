using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class RetrievePlanSongDataService : IRetrievePlanSongDataService
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly ITransformData _transformData;
        private readonly IRetrieveItemsService _retrieveItemsService;
        private readonly IDatabaseUpdateService _databaseUpdateService;

        public RetrievePlanSongDataService(ILogger<RetrievePlanningCenterDataService> log,
            ITransformData transformData,
            IRetrieveItemsService retrieveItemsService,
            IDatabaseUpdateService databaseUpdateService)
        {
            _log = log;
            _transformData = transformData;
            _retrieveItemsService = retrieveItemsService;
            _databaseUpdateService = databaseUpdateService;
        }

        public async Task GetPlanSongDataForSpecifiedPlanAsync(AvailablePlansInformation plansInformation, Plan plan)
        {
            if(plan.attributes.sort_date >= DateTime.Now.AddDays(-plansInformation.RefreshAppConfig.NumberOfDaysToRefreshPlanSongData)
                    && plan.attributes.sort_date < DateTime.Now.AddDays(plansInformation.RefreshAppConfig.NumberOfDaysToRefreshFutureData))
           {
                Plan_ForSongs plan_ForSongs = new Plan_ForSongs();
                plan_ForSongs.PlanId = plan.id;
                plan_ForSongs.PlanDateTime = plan.attributes.sort_date;
                plan_ForSongs.Type = plansInformation.CurrentPlanType;
                //_log.LogInformation("--------------------------------------------------------------------------------------------------");
                //_log.LogInformation("--------------------------------------------------------------------------------------------------");
                //_log.LogInformation("Retrieving Plan Song Data for Date/Time: {dateTime}", plan.attributes.sort_date);
                _retrieveItemsService.GetItemsForSpecifiedPlan(plansInformation, plan);
                plansInformation.NumberofPlansUsedToRefreshGeneralSongs++;
                foreach (Item item in plansInformation.CurrentRetrievedItems.data)
                {
                    if (item.attributes.item_type.ToLower().Equals(ServiceConstants.SONG))
                    {
                        PlanSong song = getPlanSongRecord(plansInformation, plan, item);
                        await getSongDetailsAsync(plansInformation, plan, item, song);
                        plan_ForSongs.PlanSongs.Add(song);
                    }
                }
                plansInformation.Plan_ForSongsList.Add(plan_ForSongs);
            }
        }

        private PlanSong getPlanSongRecord(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            //_log.LogInformation("--------------------------------------------------------------------------------------------------");
            //_log.LogInformation("Song Information:");

            PlanSong planSong = new();
            planSong.SongId = item.relationships.song.data.id;
            planSong.KeyName = item.attributes.key_name;
            planSong.Description = item.attributes.description;
            planSong.Sequence = item.attributes.sequence;

            //logItem(item);
            Song song = _retrieveItemsService.GetSongRecord(plansInformation, item);
            //logSong(song);
            planSong.Author = song.data.attributes.author;
            planSong.Copyright = song.data.attributes.copyright;
            planSong.SongName = song.data.attributes.title;

            Arrangement arrangement = getArrangementRecord(plansInformation, plan, item);
            if (arrangement.data != null)
            {
                //logArrangement(arrangement);
                planSong.ArrangementId = arrangement.data.id;
                planSong.ArrangementName = arrangement.data.attributes.name;
                planSong.Length = arrangement.data.attributes.length;
                planSong.Notes = arrangement.data.attributes.notes;
            }
            else
            {
                planSong.ArrangementId = item.relationships.song.data.id;
                planSong.ArrangementName = item.attributes.title != null ? item.attributes.title : "There is no Arrangement Name available.";
                planSong.Length = item.attributes.length;
            }

            return planSong;
        }

        private Arrangement getArrangementRecord(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            string getSpecificPlan = $"{plansInformation.CurrentUrl}/{plan.id}/items";
            string getArrangementUrl = $"{getSpecificPlan}/{item.id}/arrangement";
            var responseTask = plansInformation.Client.GetAsync(getArrangementUrl);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            if (plansInformation.NumberOfRequests > plansInformation.PlanningCtrConfig.RateLimit - 5)
            {
                Thread.Sleep(1000 * plansInformation.PlanningCtrConfig.RatePeriod);
                plansInformation.NumberOfRequests = 0;
                readTask.Wait();
            }
            else
            {
                readTask.Wait();
            }
            string attachmentResults = readTask.Result;
            return JsonConvert.DeserializeObject<Arrangement>(attachmentResults);
        }

        private void logItem(Item item)
        {
            _log.LogInformation("Item Data:");
            _log.LogInformation("Song ID: {songId}", item.relationships.song.data.id);
            _log.LogInformation("Description: {desc}", item.attributes.description);
            _log.LogInformation("Key Name: {keyName}", item.attributes.key_name);
            _log.LogInformation("Sequence: {sequence}", item.attributes.sequence);
        }

        private void logSong(Song song)
        {
            _log.LogInformation("Song Data:");
            _log.LogInformation("Song Name: {songName}", song.data.attributes.title);
            _log.LogInformation("Author: {songAuthor}", song.data.attributes.author);
            _log.LogInformation("Copyright: {songCopyright}", song.data.attributes.copyright);

        }

        private void logArrangement(Arrangement arrangement)
        {
            _log.LogInformation("Arrangement Data:");
            _log.LogInformation("Arrangement ID: {arrangmentId}", arrangement.data.id);
            _log.LogInformation("Arrangement Name: {arrangementName}", arrangement.data.attributes.name);
            _log.LogInformation("Arrangement Length: {arrangmentLength}", arrangement.data.attributes.length);
            _log.LogInformation("Arrangement Notes: {arrangementNotes}", arrangement.data.attributes.notes);
        }

        private async Task getSongDetailsAsync(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item,
            PlanSong song)
        {
            getSongNotes(plansInformation, plan, item, song);
            await getSongAttachmentsAsync(plansInformation, plan, item, song);
        }

        
        private void getSongNotes(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item,
            PlanSong song
            )
        {
            string getSpecificPlan = $"{plansInformation.CurrentUrl}/{plan.id}/items";
            string getItemNotesUrl = $"{getSpecificPlan}/{item.id}/item_notes";

            // Retrieve sermon notes.
            var responseTask = plansInformation.Client.GetAsync(getItemNotesUrl);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            if (plansInformation.NumberOfRequests > plansInformation.PlanningCtrConfig.RateLimit - 5)
            {
                Thread.Sleep(1000 * plansInformation.PlanningCtrConfig.RatePeriod);
                plansInformation.NumberOfRequests = 0;
                readTask.Wait();
            }
            else
            {
                readTask.Wait();
            }

            string itemNotesResults = readTask.Result;
            plansInformation.CurrentRetrievedItemNotes = JsonConvert.DeserializeObject<ItemNotes>(itemNotesResults);
            if (plansInformation.CurrentRetrievedItemNotes.data != null)
            {
                //_log.LogInformation("Song note information:");
                int count = 0;
                foreach (ItemNote note in plansInformation.CurrentRetrievedItemNotes.data)
                {
                    count++;
                    //_log.LogInformation("Item Note {itemCount} Category Name: {itemNotesName}", count, note.attributes.category_name);
                    //_log.LogInformation("Item Note {itemCount} Category Content: {itemNotesName}", count, note.attributes.content);
                    SongNote songNote = new SongNote();
                    songNote.CategoryName = note.attributes.category_name;
                    songNote.Content = note.attributes.content;
                    song.SongNotes.Add(songNote);
                }
            }
        }

        private async Task getSongAttachmentsAsync(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item,
            PlanSong planSong)
        {
            string getSpecificPlan = $"{plansInformation.CurrentUrl}/{plan.id}/items";
            string getAttachmentsUrl = $"{getSpecificPlan}/{item.id}/attachments";

            // Retrieve sermon attachments.

            var responseTask = plansInformation.Client.GetAsync(getAttachmentsUrl);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            if (plansInformation.NumberOfRequests > plansInformation.PlanningCtrConfig.RateLimit - 5)
            {
                //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
                Thread.Sleep(1000 * plansInformation.PlanningCtrConfig.RatePeriod);
                plansInformation.NumberOfRequests = 0;
                readTask.Wait();
            }
            else
            {
                readTask.Wait();
            }
            string attachmentResults = readTask.Result;
            try
            {
                plansInformation.CurrentRetrievedSongAttachments = JsonConvert.DeserializeObject<SongAttachments>(attachmentResults);
            }
            catch (Exception ex)
            {
                plansInformation.CurrentRetrievedAttachments.data = null;
                //_log.LogInformation("*********************************************************************************");
                //_log.LogInformation("*********************************************************************************");
                //_log.LogInformation("There was an error deserializing the attachment results for {song}:", item.attributes.title);
                //_log.LogInformation("URL: {url}", getAttachmentsUrl);
                //_log.LogInformation("ERROR:{error}", ex);
                //_log.LogInformation("*********************************************************************************");
                //_log.LogInformation("*********************************************************************************");
            }

            if (plansInformation.CurrentRetrievedSongAttachments.data != null)
            {
                int count = 0;
                //_log.LogInformation("Song Attachment Information:");
                foreach (Models.PlanningCenter.SongAttachment attachment in plansInformation.CurrentRetrievedSongAttachments.data)
                {
                    count++;
                    attachment.attributes.filename = getFileName(attachment.attributes.filename);
                    if (attachment.attributes.filetype == "pdf" || attachment.attributes.filetype == "file") 
                    {
                        await getPdfOrFileAttachmentAsync(plansInformation, attachment, planSong);
                    }
                    else if(attachment.attributes.filetype == "audio" || attachment.attributes.filetype == "video")
                    {
                        await getMp3OrMp4AttachmentAsync(plansInformation, attachment, planSong);
                        //_log.LogInformation("URL:{url}", attachment.attributes.url);
                    }
                }
            }
        }


        private async Task getPdfOrFileAttachmentAsync(
            AvailablePlansInformation plansInformation,
            Models.PlanningCenter.SongAttachment attachment,
            PlanSong planSong
        )
        {
            if (attachment.attributes.downloadable && attachment.attributes.file_size > 0)
            {
                byte[] file;
                HttpContent c = new StringContent("{ }", Encoding.UTF8, "application/json");
                var fileResponseTask = plansInformation.Client.PostAsync($"https://api.planningcenteronline.com/services/v2/attachments/{attachment.id}/open", c);
                plansInformation.NumberOfRequests++;
                fileResponseTask.Wait();
                var fileResponseStringTask = fileResponseTask.Result.Content.ReadAsStringAsync();
                if (plansInformation.NumberOfRequests > plansInformation.PlanningCtrConfig.RateLimit - 5)
                {
                    Thread.Sleep(1000 * plansInformation.PlanningCtrConfig.RatePeriod);
                    plansInformation.NumberOfRequests = 0;
                    fileResponseStringTask.Wait();
                }
                else
                {
                    fileResponseStringTask.Wait();
                }

                string attachmentTypeResults = fileResponseStringTask.Result;
                AttachmentActivityResult attachmentActivityResult = JsonConvert.DeserializeObject<AttachmentActivityResult>(attachmentTypeResults);
                HttpClient client = new HttpClient();
                var actualFileResponseTask = client.GetStreamAsync(attachmentActivityResult.data.attributes.attachment_url);
                plansInformation.NumberOfRequests++;
                actualFileResponseTask.Wait();
                MemoryStream ms = new MemoryStream();

                await actualFileResponseTask.Result.CopyToAsync(ms);
                file = ms.ToArray();
                Domain.SongAttachment newAttachmentRecord = new Domain.SongAttachment();
                newAttachmentRecord.FileName = attachment.attributes.filename;
                newAttachmentRecord.ContentType = attachment.attributes.content_type;
                newAttachmentRecord.Downloadable = attachment.attributes.downloadable;
                newAttachmentRecord.File = file;
                newAttachmentRecord.FileSize = attachment.attributes.file_size;
                newAttachmentRecord.FileType = attachment.attributes.filetype;
                newAttachmentRecord.HasPreview = attachment.attributes.has_preview;
                newAttachmentRecord.Url = attachment.attributes.url;
                //_log.LogInformation("Retrieved a PDF or File to store in the database.  File Name:{fileName}", attachment.attributes.filename);
                planSong.SongAttachments.Add(newAttachmentRecord);
            }
        }

        private async Task getMp3OrMp4AttachmentAsync(
            AvailablePlansInformation plansInformation,
            Models.PlanningCenter.SongAttachment attachment,
            PlanSong planSong
        )
        {
            if (attachment.attributes.downloadable && attachment.attributes.file_size > 0)
            {
                byte[] file;
                HttpContent c = new StringContent("{ }", Encoding.UTF8, "application/json");
                var fileResponseTask = plansInformation.Client.PostAsync($"https://api.planningcenteronline.com/services/v2/attachments/{attachment.id}/open", c);
                plansInformation.NumberOfRequests++;
                fileResponseTask.Wait();
                var fileResponseStringTask = fileResponseTask.Result.Content.ReadAsStringAsync();
                if (plansInformation.NumberOfRequests > plansInformation.PlanningCtrConfig.RateLimit - 5)
                {
                    Thread.Sleep(1000 * plansInformation.PlanningCtrConfig.RatePeriod);
                    plansInformation.NumberOfRequests = 0;
                    fileResponseStringTask.Wait();
                }
                else
                {
                    fileResponseStringTask.Wait();
                }

                string attachmentTypeResults = fileResponseStringTask.Result;
                AttachmentActivityResult attachmentActivityResult = JsonConvert.DeserializeObject<AttachmentActivityResult>(attachmentTypeResults);
                HttpClient client = new HttpClient();
                var actualFileResponseTask = client.GetStreamAsync(attachmentActivityResult.data.attributes.attachment_url);
                plansInformation.NumberOfRequests++;
                actualFileResponseTask.Wait();
                MemoryStream ms = new MemoryStream();

                await actualFileResponseTask.Result.CopyToAsync(ms);
                file = ms.ToArray();
                Domain.SongAttachment newAttachmentRecord = new Domain.SongAttachment();
                newAttachmentRecord.FileName = attachment.attributes.filename;
                newAttachmentRecord.ContentType = attachment.attributes.content_type;
                newAttachmentRecord.Downloadable = attachment.attributes.downloadable;
                newAttachmentRecord.FilePath = ServiceConstants.FILE_PATH_SONG_AUDIO + attachment.attributes.filename;
                newAttachmentRecord.FileSize = attachment.attributes.file_size;
                newAttachmentRecord.FileType = attachment.attributes.filetype;
                newAttachmentRecord.HasPreview = attachment.attributes.has_preview;
                newAttachmentRecord.Url = attachment.attributes.url;
                await File.WriteAllBytesAsync(ServiceConstants.FILE_PATH_SONG_AUDIO + attachment.attributes.filename, file);
                //_log.LogInformation("Retrieved and saved an MP3 or MP4 to the hard drive.  File Name:{fileName}", attachment.attributes.filename);
                planSong.SongAttachments.Add(newAttachmentRecord);
            }
        }

        private string getFileName(string originalFileName)
        {
            string newFileName = originalFileName;
            if (string.IsNullOrEmpty(originalFileName))
            {
                newFileName = "originalFileNameWasInvalid";
            }
            if (originalFileName.Contains('?'))
            {
                newFileName = originalFileName.Replace('?', '-');
                originalFileName = newFileName;
            }
            if (originalFileName.Contains('&'))
            {
                newFileName = originalFileName.Replace('&', '-');
                originalFileName = newFileName;
            }
            if (originalFileName.Contains(','))
            {
                newFileName = originalFileName.Replace(',', '-');
                originalFileName = newFileName;
            }
            if (originalFileName.Contains('>'))
            {
                newFileName = originalFileName.Replace('>', '-');
                originalFileName = newFileName;
            }
            if (originalFileName.Contains('<'))
            {
                newFileName = originalFileName.Replace('<', '-');
                //originalFileName = newFileName;
            }

            return newFileName;
        }
    }
}
