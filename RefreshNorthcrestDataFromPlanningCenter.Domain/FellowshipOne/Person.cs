using System;
using System.Collections.Generic;

namespace Northcrest.Domain.FellowshipOne
{
    public class Person
    {
        public Person()
        {
            Events = new List<Event>();
            Addresses = new List<Address>();
            Communications = new List<Communication>();
            BackgroundChecks = new List<BackgroundCheck>();
        }
        public int PersonId { get; set; }
        public int FellowshipOneId  { get; set; }
        public int HouseholdId { get; set; }
        public string HouseholdMemberType { get; set; }
        public string ImageFilePath { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string MiddleName { get; set; }
        public string GoesByName { get; set; }
        public string FormerName { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string FormerChurch { get; set; }
        public string Employer { get; set; }
        public string Denomination { get; set; }
        public string OccupationName { get; set; }
        public string OccupationDescription { get; set; }
        public string School { get; set; }
        public string Status { get; set; }
        public string Substatus { get; set; }
        public string StatusComment { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastMatchDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime FirstRecordDate { get; set; }
        

        public List<Event> Events { get; set; }
        public List<Address> Addresses { get; set; }
        public List<Communication> Communications {  get; set; }
        public List<BackgroundCheck> BackgroundChecks { get; set; }
        //public PersonAddresses? addresses { get; set; }
        //public PersonCommunications? communications { get; set; }
        //public PersonRequirements? peopleRequirements { get; set; }


    }
}
