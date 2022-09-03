using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientSermonFactory
    {
        IClientSermon CreateEmptyClientSermon();
        IClientSermon CreateClientSermonFromSermon(Sermon sermon);
        IList<IClientSermon> CreateClientSermonList();
    }
}
