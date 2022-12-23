using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Models.Interfaces
{
    public interface IClientPlan_ForSongs
    {
        int Plan_ForSongsId { get; set; }
        int PlanId { get; set; }
        string Type { get; set; }
        DateTime PlanDateTime { get; set; }
        List<IClientPlanSong> PlanSongs { get; set; }
    }
}
