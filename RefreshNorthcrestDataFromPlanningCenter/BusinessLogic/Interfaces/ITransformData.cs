using RefreshNorthcrestDataFromPlanningCenter.Domain;
using RefreshNorthcrestDataFromPlanningCenter.Models.PlanningCenter;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public interface ITransformData
    {
        void ExecuteSermonDataCorrections (Sermon sermon);
        void ExecuteGeneralSongDataCorrections(GeneralSong generalSong);
    }
}
