using Northcrest.Domain.PlanningCenter;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public interface ITransformData
    {
        void ExecuteSermonDataCorrections (Sermon sermon);
        void ExecuteGeneralSongDataCorrections(GeneralSong generalSong);
    }
}
