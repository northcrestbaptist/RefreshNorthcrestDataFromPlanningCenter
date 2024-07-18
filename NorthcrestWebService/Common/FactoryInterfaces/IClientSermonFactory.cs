using Northcrest.Domain.PlanningCenter;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Common.FactoryInterfaces
{
    public interface IClientSermonFactory
    {
        IClientSermon CreateEmptyClientSermon();
        IClientSermon CreateClientSermonFromSermon(Sermon sermon);
        IList<IClientSermon> CreateClientSermonList();
    }
}
