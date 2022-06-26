using System.Collections.Generic;
using System.Threading.Tasks;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.App_BusinessLogic.Interfaces
{
    public interface ISermonProcessor
    {
        Task<IList<Sermon>> GetSermonsAsync();
    }
}