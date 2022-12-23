using NorthcrestWebService.Common.FactoryInterfaces;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Common.Factories
{
    public class ClientPlan_ForSongsFactory : IClientPlan_ForSongsFactory
    {
        private readonly IClientSongFactory _clientSongFactory;

        public ClientPlan_ForSongsFactory(IClientSongFactory clientSongFactory)
        {
            _clientSongFactory = clientSongFactory;
        }

        public IClientPlan_ForSongs CreateEmptyClientPlan_ForSongs()
        {
            return new ClientPlan_ForSongs();
        }

        public IClientPlan_ForSongs CreateClientPlan_ForSongsFromPlan_ForSongs(Plan_ForSongs plan_ForSongs)
        {
            IClientPlan_ForSongs clientPlan_ForSongs = CreateEmptyClientPlan_ForSongs();
            clientPlan_ForSongs.Plan_ForSongsId = plan_ForSongs.Plan_ForSongsId;
            clientPlan_ForSongs.PlanDateTime = plan_ForSongs.PlanDateTime;
            clientPlan_ForSongs.PlanId = plan_ForSongs.PlanId;
            clientPlan_ForSongs.Type = plan_ForSongs.Type;
            foreach (PlanSong planSong in plan_ForSongs.PlanSongs)
            {
                IClientPlanSong clientPlanSong = _clientSongFactory.CreateClientPlanSongFromPlanSong(planSong);
                clientPlan_ForSongs.PlanSongs.Add(clientPlanSong);
            }
            return clientPlan_ForSongs;
        }

        public IList<IClientPlan_ForSongs> CreateClientPlan_ForSongsList()
        {
            return new List<IClientPlan_ForSongs>();
        }
    }
}
