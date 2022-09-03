using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic
{
    public class GetSermonPlanDetails : IGetSermonPlanDetails
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly ITransformData _transformData;

        public GetSermonPlanDetails(
            ILogger<RetrievePlanningCenterDataService> log, 
            ITransformData transformData)
        {
            _log = log;
            _transformData = transformData; 
        }

        public void GetDetailsForAllPlansForServiceType(AvailablePlansInformation plansInformation)
        {
            while (plansInformation.RemainingRecords > 0)
            {
                GetNextPageOfPlans(plansInformation);
                foreach (Plan plan in plansInformation.CurrentRetrievedPlans.data)
                {
                    if (plan.attributes.sort_date > plansInformation.MostRecentSermonInNorthcrestDatabase
                    && plan.attributes.sort_date < DateTime.Now
                    && !DoesPlanIdExistAlready(plansInformation, plan.id))
                    {
                        GetItemsForPlan(plansInformation, plan);
                        foreach (Item item in plansInformation.CurrentRetrievedItems.data)
                        {
                            if (item.attributes.title.ToLower().StartsWith(ServiceConstants.SERMON))
                            {
                                GetSermonDetails(plansInformation, plan, item);
                            }

                        }
                    }
                }
                plansInformation.RemainingRecords -= plansInformation.Configuration.RateLimit;
                plansInformation.OffSet += plansInformation.Configuration.RateLimit;
            }
        }

        private void GetNextPageOfPlans(AvailablePlansInformation plansInformation)
        {
            string getPlansByPage =
                    $"{plansInformation.Url}?offset={plansInformation.OffSet}&per_page={plansInformation.Configuration.RateLimit}";
           
            plansInformation.NumberOfRequests++;
            var responseTask = plansInformation.Client.GetAsync(getPlansByPage);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();
            string planningCenterResults = readTask.Result;
            plansInformation.CurrentRetrievedPlans = JsonConvert.DeserializeObject<Plans>(planningCenterResults);
        }

        private bool DoesPlanIdExistAlready(AvailablePlansInformation plansInformation, int planId)
        {
            return plansInformation.Sermons.Any(x => x.SermonId == planId);
        }

        private void GetItemsForPlan(AvailablePlansInformation plansInformation, Plan plan)
        {
            //_log.LogInformation("Retrieving items for: {planID}", plan.id);
            string getSpecificPlan = $"{plansInformation.Url}/{plan.id}/items";
            var responseTask = plansInformation.Client.GetAsync(getSpecificPlan);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();

            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            if (plansInformation.NumberOfRequests > plansInformation.Configuration.RateLimit - 5)
            {
                //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
                Thread.Sleep(1000 * plansInformation.Configuration.RatePeriod);
                plansInformation.NumberOfRequests = 0;
                readTask.Wait();
            }
            else
            {
                readTask.Wait();
            }

            string planItemResults = readTask.Result;

            plansInformation.CurrentRetrievedItems = JsonConvert.DeserializeObject<Items>(planItemResults);
        }

        private async void GetSermonDetails(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            Sermon sermon = new();
            sermon.PlanId = plan.id;
            sermon.Type = plansInformation.Type;
            sermon.SermonDateTime = plan.attributes.sort_date;
            sermon.Title = item.attributes.title;
            sermon.Description = item.attributes.description;


            getSermonNotes(plansInformation, plan, item, sermon);
            await getSermonAttachments(plansInformation, plan, item, sermon);
            addSermonToList(plansInformation, sermon);
            
        }

        private void addSermonToList(AvailablePlansInformation plansInformation, Sermon sermon)
        {
            if (!DoesRecordHaveNoData(sermon))
            {
                _transformData.ExecuteDataCorrections(sermon);
                plansInformation.Sermons.Add(sermon);
                _log.LogInformation("Added the following sermon plan information to the queue for addding to the Northcrest database:");
                _log.LogInformation("Plan ID: {planId}", sermon.PlanId);
                _log.LogInformation("Service Type: {serviceType}", sermon.Type);
                _log.LogInformation("Sermon Date/Time: {serviceDate}", sermon.SermonDateTime);
                _log.LogInformation("Speaker: {speaker}", sermon.Speaker);
                _log.LogInformation("Sermon Title: {sermonTitle}", sermon.Title);
                _log.LogInformation("Sermon Description: {sermonDescription}", sermon.Description);
                _log.LogInformation("Number of sermon attachments: {attachmentCount}", sermon.Attachments.Count);
                if (sermon.Attachments.Count > 0)
                {
                    _log.LogInformation("Attachment Record Info:...");
                }
                int attachmentRecordCount = 0;
                foreach (Domain.Attachment attachment in sermon.Attachments)
                {
                    _log.LogInformation("Attachment record {recordCount}:", attachmentRecordCount++);
                    _log.LogInformation("Attachment ID: {id}", attachment.AttachmentId);
                    _log.LogInformation("Attachment File Name: {fileName}", attachment.FileName);
                    _log.LogInformation("Attachment File Type: {fileType}", attachment.FileType);
                    _log.LogInformation("Attachment Content Type: {contentType}", attachment.ContentType);
                    _log.LogInformation("Attachment File Size: {fileSize}", attachment.FileSize);
                    _log.LogInformation("Url for downloading attachment: {url}", attachment.Url);
                    _log.LogInformation("Attachment Downloadable? {downloadable}", attachment.Downloadable);
                    _log.LogInformation("Has Preview?: {hasPreview}", attachment.HasPreview);
                }
            }
        }

        private void getSermonNotes(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item,
            Sermon sermon)
        {
            string getSpecificPlan = $"{plansInformation.Url}/{plan.id}/items";
            string getItemNotesUrl = $"{getSpecificPlan}/{item.id}/item_notes";

            // Retrieve sermon notes.
            var responseTask = plansInformation.Client.GetAsync(getItemNotesUrl);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            if (plansInformation.NumberOfRequests > plansInformation.Configuration.RateLimit - 5)
            {
                //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
                Thread.Sleep(1000 * plansInformation.Configuration.RatePeriod);
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

        private async Task getSermonAttachments(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item,
            Sermon sermon)
        {
            string getSpecificPlan = $"{plansInformation.Url}/{plan.id}/items";
            string getAttachmentsUrl = $"{getSpecificPlan}/{item.id}/attachments";

            // Retrieve sermon attachments.
            byte[] file;
            var responseTask = plansInformation.Client.GetAsync(getAttachmentsUrl);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            if (plansInformation.NumberOfRequests > plansInformation.Configuration.RateLimit - 5)
            {
                //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
                Thread.Sleep(1000 * plansInformation.Configuration.RatePeriod);
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
                foreach (RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter.Attachment attachment in plansInformation.CurrentRetievedAttachments.data)
                {
                    attachment.attributes.filename = getFileName(attachment.attributes.filename);
                    if ((attachment.attributes.filetype == "pdf") //|| attachment.attributes.filetype == "video")
                        && attachment.attributes.downloadable
                        && attachment.attributes.file_size > 0)
                    {
                        HttpContent c = new StringContent("{ }", Encoding.UTF8, "application/json");
                        var fileResponseTask = plansInformation.Client.PostAsync($"https://api.planningcenteronline.com/services/v2/attachments/{attachment.id}/open", c);
                        plansInformation.NumberOfRequests++;
                        fileResponseTask.Wait();
                        var fileResponseStringTask = fileResponseTask.Result.Content.ReadAsStringAsync();
                        if (plansInformation.NumberOfRequests > plansInformation.Configuration.RateLimit - 5)
                        {
                            //_log.LogInformation("Pausing program execution to adhere to Planning Center web api request limits.");
                            Thread.Sleep(1000 * plansInformation.Configuration.RatePeriod);
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
            }
        }

        private string getFileName(string originalFileName)
        {
            string newFileName = originalFileName;
            if(string.IsNullOrEmpty(originalFileName))
            {
                newFileName = "originalFileNameWasInvalid";
            } else if(originalFileName.Contains('?'))
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
