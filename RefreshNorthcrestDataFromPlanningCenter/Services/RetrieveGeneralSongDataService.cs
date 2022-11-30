using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.Threading;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class RetrieveGeneralSongDataService: IRetrieveGeneralSongDataService
    {
        private readonly ILogger<RetrievePlanningCenterDataService> _log;
        private readonly ITransformData _transformData;
        private readonly IRetrieveItemsService _retrieveItemsService;
        private readonly IDatabaseUpdateService _databaseUpdateService;

        public RetrieveGeneralSongDataService(ILogger<RetrievePlanningCenterDataService> log,
            ITransformData transformData,
            IRetrieveItemsService retrieveItemsService,
            IDatabaseUpdateService databaseUpdateService)
        {
            _log = log;
            _transformData = transformData;
            _retrieveItemsService = retrieveItemsService;
            _databaseUpdateService = databaseUpdateService;
        }

        public void GetGeneralSongDataForSpecifiedPlan(AvailablePlansInformation plansInformation, Plan plan)
        {
            // Change to most recent general song in northcrest database once table exists
            if (plan.attributes.sort_date >= DateTime.Now.AddDays(-plansInformation.RefreshAppConfig.NumberOfDaysToRefreshGeneralSongData)
                    && plan.attributes.sort_date < DateTime.Now.AddDays(plansInformation.RefreshAppConfig.NumberOfDaysToRefreshFutureData))
            {
                //_log.LogInformation("--------------------------------------------------------------------------------------------------");
                //_log.LogInformation("--------------------------------------------------------------------------------------------------");
                //_log.LogInformation("Retrieving General Song Data for Date/Time: {dateTime}", plan.attributes.sort_date);
                _retrieveItemsService.GetItemsForSpecifiedPlan(plansInformation, plan);
                plansInformation.NumberofPlansUsedToRefreshGeneralSongs++;
                foreach (Item item in plansInformation.CurrentRetrievedItems.data)
                {
                    if (item.attributes.item_type.ToLower().Equals(ServiceConstants.SONG))
                    {
                        GeneralSong song = getGeneralSongRecord(plansInformation, plan, item);
                        _databaseUpdateService.AddOrUpdateGeneralSong(song);
                        plansInformation.NumberOfGeneralSongsRefreshed++;
                    }
                }
            }
        }

        private GeneralSong getGeneralSongRecord(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            //_log.LogInformation("--------------------------------------------------------------------------------------------------");
            //_log.LogInformation("Song Information:");
            
            GeneralSong generalSong = new();
            generalSong.SongId = item.relationships.song.data.id;
            //logItem(item);
            Song song = getSongRecord(plansInformation, item);
            //logSong(song);
            generalSong.Author = song.data.attributes.author;
            //generalSong.CcliNumber = song.data.attributes.ccli_number;
            generalSong.Copyright = song.data.attributes.copyright;
            generalSong.LastScheduledDateTime = song.data.attributes.last_scheduled_at;
            generalSong.SongName = song.data.attributes.title;
            generalSong.Themes = song.data.attributes.themes;
            Arrangement arrangement = getArrangementRecord(plansInformation, plan, item);
            if(arrangement.data != null)
            {
                //logArrangement(arrangement);
                generalSong.ArrangementId = arrangement.data.id;
                generalSong.ArrangementName = arrangement.data.attributes.name;
                generalSong.Length = arrangement.data.attributes.length;
            }
            else
            {
                generalSong.ArrangementId = item.relationships.song.data.id;
                generalSong.ArrangementName = item.attributes.title != null ? item.attributes.title : "There is no Arrangement Name available.";
                generalSong.Length = item.attributes.length;
            }
            _transformData.ExecuteGeneralSongDataCorrections(generalSong);
            return generalSong;
        }
        
        private Song getSongRecord(
            AvailablePlansInformation plansInformation,
            Item item
            )
        {
            string getSongRecord = $"https://api.planningcenteronline.com/services/v2/songs/{item.relationships.song.data.id}";
            //_log.LogInformation("getSongRecordUrl: {url}", getSongRecord);
            //_log.LogInformation("Item Relationships Debug info: {relationships}", item.relationships);
            // Retrieve song record.
            var responseTask = plansInformation.Client.GetAsync(getSongRecord);
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

            string songRecordResults = readTask.Result;


            return JsonConvert.DeserializeObject<Song>(songRecordResults);
        }

        private Arrangement getArrangementRecord(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            string getSpecificPlan = $"{plansInformation.CurrentUrl}/{plan.id}/items";
            string getArrangementUrl = $"{getSpecificPlan}/{item.id}/arrangement";

            // Retrieve sermon attachments.

            var responseTask = plansInformation.Client.GetAsync(getArrangementUrl);
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
            return JsonConvert.DeserializeObject<Arrangement>(attachmentResults);
        }

        private void logItem(Item item)
        {
            _log.LogInformation("Item Data:");
            _log.LogInformation("Song ID: {songId}", item.relationships.song.data.id);
        }

        private void logSong(Song song)
        {
            _log.LogInformation("Song Data:");
            _log.LogInformation("Song Name: {songName}", song.data.attributes.title); 
            _log.LogInformation("Author: {songAuthor}", song.data.attributes.author);
            //_log.LogInformation("CCLI Number: {songCcliNumber}", song.data.attributes.ccli_number);
            _log.LogInformation("Copyright: {songCopyright}", song.data.attributes.copyright);
            _log.LogInformation("Last Scheduled Date/Time: {songLastScheduledDateTime}", song.data.attributes.last_scheduled_at);
            
        }

        private void logArrangement(Arrangement arrangement)
        {
            _log.LogInformation("Arrangement Data:");
            _log.LogInformation("Arrangement ID: {arrangmentId}", arrangement.data.id);
            _log.LogInformation("Arrangement Name: {arrangementName}", arrangement.data.attributes.name);
            _log.LogInformation("Arrangement Length: {arrangmentLength}", arrangement.data.attributes.length);
            _log.LogInformation("Arrangement Notes: {arrangementNotes}", arrangement.data.attributes.notes);
        }
    }
}
