using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace RefreshNorthcrestDataFromPlanningCenter.BusinessLogic.Interfaces
{
    public interface ITransformData
    {
        void ExecuteDataCorrections (Sermon sermon);
    }
}
