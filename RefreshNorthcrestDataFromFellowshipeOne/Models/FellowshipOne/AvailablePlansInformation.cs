using Microsoft.Data.SqlClient.DataClassification;
using RefreshNorthcrestDataFromFellowshipOne.Models.Local;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class AvailablePlansInformation
    {
        //public DateTime MostRecentSermonInNorthcrestDatabase { get; set; }
        //public DateTime MostRecentGeneralSongInfoInNorthcrestDatabase { get; set; }
        //public DateTime MostRecentPlanWithSongInfoInNorthcrestDatabase { get; set; }
        public HttpClient Client { get; set; }
        //public string[] PlanUrlList { get; set; }
        //public string[] PlanTypeList { get; set; } = new string[3];
        public LocalAppConfiguration RefreshAppConfig {get; set; }
        public FellowshipOneConfiguration PlanningCtrConfig { get; set; }
        public string CurrentUrl { get; set; }
        public int NumberOfRequests { get; set; }
        //public int PlanIndex { get; set; }
        public string CurrentPlanType { get; set; }
        //public int TotalRecordsAvailable { get; set; }
        //public int RemainingRecords { get; set; }
        //public int OffSet { get; set; } = 0;
        //public int NumberofPlansUsedToRefreshGeneralSongs { get; set; } = 0;
        //public int NumberOfGeneralSongsRefreshed { get; set; } = 0;
        //public Plans CurrentRetrievedPlans { get; set; } 
        //public Items CurrentRetrievedItems { get; set; }
        //public ItemNotes CurrentRetrievedItemNotes { get; set; }
        //public Attachments CurrentRetrievedAttachments { get; set; }
        //public SongAttachments CurrentRetrievedSongAttachments { get; set; }
        //public IList<Sermon> Sermons { get; set; } = new List<Sermon>();
        //public IList<Plan_ForSongs> Plan_ForSongsList { get; set; } = new List<Plan_ForSongs>();

    }
}
