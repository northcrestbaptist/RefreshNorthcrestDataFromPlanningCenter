using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System;
using System.Collections.Generic;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public  interface INorthcrestLocalData
    {
        DateTime GetLatestSermonDateTime();
        void AddSermonsToDatabase(IList<Sermon> sermons);
    }
}
