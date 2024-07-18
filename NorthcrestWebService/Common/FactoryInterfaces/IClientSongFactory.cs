using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.Models.Interfaces;

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
