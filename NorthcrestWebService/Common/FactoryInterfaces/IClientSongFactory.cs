using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientSongFactory
    {
        IClientGeneralSong CreateEmptyClientGeneralSong();
        IClientPlanSong CreateEmptyClientPlanSong();
        IClientGeneralSong CreateClientGeneralSongFromGeneralSong(GeneralSong generalSong);
        IClientPlanSong CreateClientPlanSongFromPlanSong(PlanSong planSong);
        IList<IClientGeneralSong> CreateClientGeneralSongList();
        IList<IClientPlanSong> CreateClientPlanSongList();
    }
}
