using Newtonsoft.Json;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class RetrieveItemsService : IRetrieveItemsService
    {
        public void GetItemsForSpecifiedPlan(AvailablePlansInformation plansInformation, Plan plan)
        {
            //_log.LogInformation("Retrieving items for: {planID}", plan.id);
            string getSpecificPlan = $"{plansInformation.CurrentUrl}/{plan.id}/items";
            var responseTask = plansInformation.Client.GetAsync(getSpecificPlan);
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

            string planItemResults = readTask.Result;

            plansInformation.CurrentRetrievedItems = JsonConvert.DeserializeObject<Items>(planItemResults);
        }

         public Song GetSongRecord(
            AvailablePlansInformation plansInformation,
            Item item
            )
        {
            string getSongRecord = $"https://api.planningcenteronline.com/services/v2/songs/{item.relationships.song.data.id}";

            // Retrieve song record.
            var responseTask = plansInformation.Client.GetAsync(getSongRecord);
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

            string songRecordResults = readTask.Result;

            return JsonConvert.DeserializeObject<Song>(songRecordResults);
        }
    }
}
