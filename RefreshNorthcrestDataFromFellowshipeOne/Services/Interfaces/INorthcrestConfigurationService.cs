using RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne;

namespace RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces
{
    public interface INorthcrestConfigurationService
    {
        void SetConfiguration(FellowshipOneInformation availablePlansInfo);
        bool IsAnythingConfiguredToRefresh(FellowshipOneInformation availablePlans);
        void LogLocalAppConfigurationInstructions();
    }
}
