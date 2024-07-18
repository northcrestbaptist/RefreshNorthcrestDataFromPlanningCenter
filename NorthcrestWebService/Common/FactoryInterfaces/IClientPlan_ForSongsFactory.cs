using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientPlan_ForSongsFactory
    {
        IClientPlan_ForSongs CreateEmptyClientPlan_ForSongs();
        IClientPlan_ForSongs CreateClientPlan_ForSongsFromPlan_ForSongs(Plan_ForSongs plan_ForSongs);
        IList<IClientPlan_ForSongs> CreateClientPlan_ForSongsList();
    }
}
