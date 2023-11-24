using RefreshNorthcrestDataFromFellowshipOne.Models.Local;
using RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces
{
    public interface INorthcrestConfigurationService
    {
        void SetConfiguration(AvailablePlansInformation availablePlansInfo);
        bool IsAnythingConfiguredToRefresh(AvailablePlansInformation availablePlans);
        void LogLocalAppConfigurationInstructions();
    }
}
