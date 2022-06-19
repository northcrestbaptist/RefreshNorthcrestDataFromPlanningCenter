using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Common.Constants;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services;
using System;
using System.Linq;
using System.Threading;

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
            //_log.LogInformation("Remaining Planning Center plans: {remainingRecords}", plansInformation.RemainingRecords);
            //_log.LogInformation("Current offset required to get next page: {offSet}", plansInformation.OffSet);
            //_log.LogInformation("Retrieving next page of plans...");
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
            return plansInformation.Sermons.Any(x => x.Id == planId);
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

        private void GetSermonDetails(
            AvailablePlansInformation plansInformation,
            Plan plan,
            Item item)
        {
            Sermon sermon = new();
            sermon.PlanID = plan.id;
            sermon.Type = plansInformation.Type;
            sermon.SermonDateTime = plan.attributes.sort_date;
            sermon.Title = item.attributes.title;
            sermon.Description = item.attributes.description;

            //_log.LogInformation("Date/Time: {serviceDate}", plan.attributes.sort_date);
            //_log.LogInformation("Title: {sermonTitle}", item.attributes.title);
            //_log.LogInformation("Description: {sermonDescription}", item.attributes.description);
            string getSpecificPlan = $"{plansInformation.Url}/{plan.id}/items";
            string getItemNotesUrl = $"{getSpecificPlan}/{item.id}/item_notes";
            var responseTask = plansInformation.Client.GetAsync(getItemNotesUrl);
            plansInformation.NumberOfRequests++;
            responseTask.Wait();
            var result = responseTask.Result;
            var readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();
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
                    _log.LogInformation("Speaker: {itemNotesName}", note.attributes.content);
                }
            }
            if (!DoesRecordHaveNoData(sermon))
            {
                _transformData.ExecuteDataCorrections(sermon);
                plansInformation.Sermons.Add(sermon);
                _log.LogInformation("Added the following sermon plan information to the queue for addding to the Northcrest database:");
                _log.LogInformation("Plan ID: {planId}", sermon.PlanID);
                _log.LogInformation("Service Type: {serviceType}", sermon.Type);
                _log.LogInformation("Sermon Date/Time: {serviceDate}", sermon.SermonDateTime);
                _log.LogInformation("Speaker: {speaker}", sermon.Speaker);
                _log.LogInformation("Sermon Title: {sermonTitle}", sermon.Title);
                _log.LogInformation("Sermon Description: {sermonDescription}", sermon.Description);
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
