using FellowshipOne.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RefreshNorthcrestDataFromFellowshipOne.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces;

namespace RefreshNorthcrestDataFromFellowshipOne.BusinessLogic
{
    public class NorthcrestLocalData : INorthcrestLocalData
    {
        private readonly ILogger<IRetrieveFellowshipOneDataService> _log;
        private readonly IConfiguration _config;
        public NorthcrestLocalData (ILogger<IRetrieveFellowshipOneDataService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public void RefreshNorthcrestDatabase(IList<Northcrest.Domain.FellowshipOne.Person> northcrestPersonnel)
        {
            using var context = new FellowshipOneDbContext();
            try
            {
                deleteAllPersonnelFromDatabase(context);
                addPersonToNorthcrestFromFellowshipOne(northcrestPersonnel, context);
                context.SaveChanges();
                _log.LogInformation("Records deleted and refreshed successfully.");
            }
            catch(Exception ex)
            {
                _log.LogInformation("There was an error: {error}", ex);
            }
        }

        private void deleteAllPersonnelFromDatabase(FellowshipOneDbContext context)
        {
            _log.LogInformation("Deleting all personnel from the database.  " +
                "Personnel will be readded using the data that was pulled from FellowshipOne during this data refresh. ");

            int numberDeleted = 0;
            var personnelToRemove = context.Persons;
            foreach (var person in personnelToRemove)
            {
                context.Persons.Remove(person);
                numberDeleted++;
            }
            _log.LogInformation("{number} [Persons] record(s) staged for deletion.", numberDeleted);
        }

        private void addPersonToNorthcrestFromFellowshipOne(
            IList<Northcrest.Domain.FellowshipOne.Person> northcrestPersonnel, 
            FellowshipOneDbContext context)
        {
            _log.LogInformation("Adding {numberOfPersonnel} personnel to the database.", northcrestPersonnel.Count);
            foreach (Northcrest.Domain.FellowshipOne.Person person in northcrestPersonnel)
            {
                context.Add(person);
            }
            _log.LogInformation("{number} [Persons] record(s) staged for adding.", northcrestPersonnel.Count);
        }
    }
}

