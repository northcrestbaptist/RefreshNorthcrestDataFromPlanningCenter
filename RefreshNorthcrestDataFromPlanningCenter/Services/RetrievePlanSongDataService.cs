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
        private readonly IConfiguration _config;
        private readonly IRetrieveItemsService _retrieveItemsService;

        public RetrievePlanSongDataService(ILogger<RetrievePlanningCenterDataService> log,
            IConfiguration config,
            IRetrieveItemsService retrieveItemsService)
        {
            _log = log;
            _config = config;
            _retrieveItemsService = retrieveItemsService;
        }

        public void GetPlanSongDataForSpecifiedPlan(AvailablePlansInformation plansInformation, Plan plan)
        {
            // Change to most recent plan song in northcrest database once table exists
            if (plan.attributes.sort_date > plansInformation.MostRecentSermonInNorthcrestDatabase
                    && plan.attributes.sort_date < DateTime.Now.AddDays(plansInformation.RefreshAppConfig.NumberOfDaysToRefreshFutureData)
                    && !DoesPlanIdExistAlready(plansInformation, plan.id))
            {
                //_log.LogInformation("--------------------------------------------------------------------------------------------------");
                //_log.LogInformation("--------------------------------------------------------------------------------------------------");
                //_log.LogInformation("Retrieving General Song Data for Date/Time: {dateTime}", plan.attributes.sort_date);
                _retrieveItemsService.GetItemsForSpecifiedPlan(plansInformation, plan);
                //foreach (Item item in plansInformation.CurrentRetrievedItems.data)
                //{
                //    if (item.attributes.item_type.ToLower().Equals(ServiceConstants.SONG))
                //    {
                //        getSongDetails(plansInformation, plan, item);
                //    }

                //}
            }
        }

        private bool DoesPlanIdExistAlready(AvailablePlansInformation plansInformation, int planId)
        {
            return plansInformation.Sermons.Any(x => x.SermonId == planId);
        }

        private async void getSermonDetails(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            Sermon sermon = new();
            sermon.PlanId = plan.id;
            sermon.Type = plansInformation.CurrentPlanType;
            sermon.SermonDateTime = plan.attributes.sort_date;
            sermon.Title = item.attributes.title;
            sermon.Description = item.attributes.description;


            getSermonNotes(plansInformation, plan, item, sermon);
            await getSermonAttachments(plansInformation, plan, item, sermon);
            addSermonToList(plansInformation, sermon);

        }

        private async void getSongDetails(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            //Sermon sermon = new();
            //sermon.PlanId = plan.id;
            //sermon.Type = plansInformation.Type;
            //sermon.SermonDateTime = plan.attributes.sort_date;
            //sermon.Title = item.attributes.title;
            //sermon.Description = item.attributes.description;
            _log.LogInformation("--------------------------------------------------------------------------------------------------");
            _log.LogInformation("Song Information:");
            _log.LogInformation("Title: {songTitle}", item.attributes.title);
            _log.LogInformation("Sequence number in service (Note from Richie: we can use this to display song order): {songId}", item.attributes.sequence);
            _log.LogInformation("Service Position (Note from Richie: this doesn't seem valuable/necessary info, as it always appears as 'during'.: {servicePosition}",
                item.attributes.service_position);
            _log.LogInformation("Description (Note from Richie: this is empty mostly, but sometimes has helpful info, so we may want this): {songDescription}",
                item.attributes.description);
            _log.LogInformation("Key Name: {keyName}", item.attributes.key_name);
            getSongNotes(plansInformation, plan, item);
            await getSongAttachments(plansInformation, plan, item);

            //addSermonToList(plansInformation, sermon);

        }

        private void addSermonToList(AvailablePlansInformation plansInformation, Sermon sermon)
        {
            if (!DoesRecordHaveNoData(sermon))
            {
                //_transformData.ExecuteDataCorrections(sermon);
                plansInformation.Sermons.Add(sermon);
                //_log.LogInformation("Added the following sermon plan information to the queue for addding to the Northcrest database:");
                //_log.LogInformation("Plan ID: {planId}", sermon.PlanId);
                //_log.LogInformation("Service Type: {serviceType}", sermon.Type);
                //_log.LogInformation("Sermon Date/Time: {serviceDate}", sermon.SermonDateTime);
                //_log.LogInformation("Speaker: {speaker}", sermon.Speaker);
                //_log.LogInformation("Sermon Title: {sermonTitle}", sermon.Title);
                //_log.LogInformation("Sermon Description: {sermonDescription}", sermon.Description);
                //_log.LogInformation("Number of sermon attachments: {attachmentCount}", sermon.Attachments.Count);
                if (sermon.Attachments.Count > 0)
                {
                    //_log.LogInformation("Attachment Record Info:...");
                }
                int attachmentRecordCount = 0;
                foreach (Domain.Attachment attachment in sermon.Attachments)
                {
                    //_log.LogInformation("Attachment record {recordCount}:", ++attachmentRecordCount);
                    //_log.LogInformation("Attachment File Name: {fileName}", attachment.FileName);
                    //_log.LogInformation("Attachment File Type: {fileType}", attachment.FileType);
                    //_log.LogInformation("Attachment Content Type: {contentType}", attachment.ContentType);
                    //_log.LogInformation("Attachment File Size: {fileSize}", attachment.FileSize);
                    //_log.LogInformation("Url for downloading attachment: {url}", attachment.Url);
                    //_log.LogInformation("Attachment Downloadable: {downloadable}", attachment.Downloadable);
                    //_log.LogInformation("Has Preview: {hasPreview}", attachment.HasPreview);
                }
            }
        }

        private void getSermonNotes(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item,
            Sermon sermon)
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
                //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
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
                foreach (ItemNote note in plansInformation.CurrentRetrievedItemNotes.data)
                {
                    if (plansInformation.CurrentRetrievedItemNotes.data.Length == 1)
                    {
                        sermon.Speaker = note.attributes.content;
                    }
                    //_log.LogInformation("Speaker: {itemNotesName}", note.attributes.content);
                }
            }
        }

        private void getSongNotes(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item
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
                //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
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
                _log.LogInformation("Song note information:");
                int count = 0;
                foreach (ItemNote note in plansInformation.CurrentRetrievedItemNotes.data)
                {
                    count++;
                    _log.LogInformation("Item Note {itemCount} Category Name: {itemNotesName}", count, note.attributes.category_name);
                    _log.LogInformation("Item Note {itemCount} Category Content: {itemNotesName}", count, note.attributes.content);
                }
            }
        }

        private async Task getSermonAttachments(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item,
            Sermon sermon)
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
            plansInformation.CurrentRetievedAttachments = JsonConvert.DeserializeObject<Attachments>(attachmentResults);
            if (plansInformation.CurrentRetievedAttachments.data != null)
            {
                foreach (Models.PlanningCenter.Attachment attachment in plansInformation.CurrentRetievedAttachments.data)
                {
                    attachment.attributes.filename = getFileName(attachment.attributes.filename);
                    if (attachment.attributes.filetype == "pdf") //|| attachment.attributes.filetype == "video")
                    {
                        await getPdfAttachment(plansInformation, attachment, sermon);
                    }
                    else if (attachment.attributes.filetype == "video")
                    {
                        getVideoAttachment(plansInformation, attachment, sermon);
                    }
                }
            }
        }

        private async Task getSongAttachments(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
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
                plansInformation.CurrentRetievedAttachments = JsonConvert.DeserializeObject<Attachments>(attachmentResults);
            }
            catch (Exception ex)
            {
                plansInformation.CurrentRetievedAttachments.data = null;
                _log.LogInformation("*********************************************************************************");
                _log.LogInformation("*********************************************************************************");
                _log.LogInformation("There was an error deserializing the attachment results for {song}:", item.attributes.title);
                _log.LogInformation("ERROR:{error}", ex);
                _log.LogInformation("*********************************************************************************");
                _log.LogInformation("*********************************************************************************");
            }

            if (plansInformation.CurrentRetievedAttachments.data != null)
            {
                int count = 0;
                _log.LogInformation("Song Attachment Information:");
                foreach (Models.PlanningCenter.Attachment attachment in plansInformation.CurrentRetievedAttachments.data)
                {
                    count++;
                    //attachment.attributes.filename = getFileName(attachment.attributes.filename);
                    //if (attachment.attributes.filetype == "pdf") //|| attachment.attributes.filetype == "video")
                    //{
                    //    await getPdfAttachment(plansInformation, attachment, sermon);
                    //}
                    //else if (attachment.attributes.filetype == "video")
                    //{
                    //    getVideoAttachment(plansInformation, attachment, sermon);
                    //}
                    _log.LogInformation("Allow mp3 download: {allowmp3}", attachment.attributes.allow_mp3_download);
                    _log.LogInformation("Content type: {contentType}", attachment.attributes.content_type);
                    _log.LogInformation("Display Name: {displayName}", attachment.attributes.display_name);
                    _log.LogInformation("Downloadable: {downloadable}", attachment.attributes.downloadable);
                    _log.LogInformation("File Size: {fileSize}", attachment.attributes.file_size);
                    _log.LogInformation("File Name: {fileName}", attachment.attributes.filename);
                    _log.LogInformation("File Type: {fileType}", attachment.attributes.filetype);
                    _log.LogInformation("Has Preview: {hasPreview}", attachment.attributes.has_preview);
                    _log.LogInformation("Thumbnail URL: {thumbnailUrl}", attachment.attributes.thumbnail_url);
                    _log.LogInformation("URL:{url}", attachment.attributes.url);
                }
            }
        }


        private async Task getPdfAttachment(
        AvailablePlansInformation plansInformation,
        Models.PlanningCenter.Attachment attachment,
        Sermon sermon)
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
                    //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
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
                Domain.Attachment newAttachmentRecord = new Domain.Attachment();
                newAttachmentRecord.FileName = attachment.attributes.filename;
                newAttachmentRecord.ContentType = attachment.attributes.content_type;
                newAttachmentRecord.Downloadable = attachment.attributes.downloadable;
                newAttachmentRecord.File = file;
                newAttachmentRecord.FileSize = attachment.attributes.file_size;
                newAttachmentRecord.FileType = attachment.attributes.filetype;
                newAttachmentRecord.HasPreview = attachment.attributes.has_preview;
                newAttachmentRecord.Url = attachment.attributes.url;
                sermon.Attachments.Add(newAttachmentRecord);
                //await File.WriteAllBytesAsync($"C:\\{attachment.attributes.filename}", file);
            }
        }

        private void getVideoAttachment(
            AvailablePlansInformation plansInformation,
            Models.PlanningCenter.Attachment attachment,
            Sermon sermon)
        {
            if (attachment.attributes.downloadable && attachment.attributes.file_size > 0)
            {
                _log.LogInformation("Found the following video file: {videoFile}", attachment.attributes.filename);
                _log.LogInformation("Development for storing video files is underway and not yet completed.");
            }
        }

        private string getFileName(string originalFileName)
        {
            string newFileName = originalFileName;
            if (string.IsNullOrEmpty(originalFileName))
            {
                newFileName = "originalFileNameWasInvalid";
            }
            else if (originalFileName.Contains('?'))
            {
                newFileName = originalFileName.Replace('?', '-');
            }

            return newFileName;
        }

        private byte[] StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }
            else
            {
                // Jon Skeet's accepted answer 
                return ReadFully(stream);
            }
        }

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private bool DoesRecordHaveNoData(Sermon sermon)
        {
            return !IsTitleValid(sermon.Title)
                && string.IsNullOrEmpty(sermon.Description)
                && string.IsNullOrEmpty(sermon.Speaker);
        }

        private bool IsTitleValid(string title)
        {
            return title != "Sermon" && title != "Sermon - ";
        }
    }
}
