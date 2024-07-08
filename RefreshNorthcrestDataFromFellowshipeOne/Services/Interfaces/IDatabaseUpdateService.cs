using RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne;

namespace RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces
{
    public interface IDatabaseUpdateService
    {
        void RefreshDataThatWasRetrievedFromFellowshipOne(FellowshipOneInformation retrievedFellowshipOneData);
    }
}
