using RefreshNorthcrestDataFromFellowshipOne.BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromFellowshipOne.Common.Constants;
using RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne;
using RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces;

namespace RefreshNorthcrestDataFromPlanningCenter.Services
{
    public class DatabaseUpdateService: IDatabaseUpdateService
    {
        private readonly INorthcrestLocalData _northcrestLocalData;

        public DatabaseUpdateService(INorthcrestLocalData northcrestLocalData)
        {
            _northcrestLocalData = northcrestLocalData;
        }

        public void RefreshDataThatWasRetrievedFromFellowshipOne(FellowshipOneInformation retrievedFellowshipOneData)
        {
            IList<Northcrest.Domain.FellowshipOne.Person> personnelToBeAddedToDatabase = new List<Northcrest.Domain.FellowshipOne.Person>();
            foreach(var person in retrievedFellowshipOneData.fellowshipOnePersonDataPull.results.person)
            {
                Northcrest.Domain.FellowshipOne.Person personToBeAddedToDatabase = new Northcrest.Domain.FellowshipOne.Person();
                personToBeAddedToDatabase.FellowshipOneId = (int)person.id;
                personToBeAddedToDatabase.HouseholdId = (int)person.householdID;
                personToBeAddedToDatabase.HouseholdMemberType =
                    person.householdMemberType != null && !string.IsNullOrEmpty(person.householdMemberType.name)
                    ? person.householdMemberType.name
                    : ServiceConstants.UNKNOWN;
                if (person.dateOfBirth != null)
                {
                    personToBeAddedToDatabase.DateOfBirth = (DateTime)person.dateOfBirth;
                }
                if (person.createdDate != null)
                {
                    personToBeAddedToDatabase.CreatedDate = (DateTime)person.createdDate;
                }
                if (person.firstRecord != null)
                {
                    personToBeAddedToDatabase.FirstRecordDate = (DateTime)person.firstRecord;
                }
                if (person.denomination != null && !string.IsNullOrEmpty(person.denomination.name))
                {
                    personToBeAddedToDatabase.Denomination = person.denomination.name;
                }
                else
                {
                    personToBeAddedToDatabase.Denomination = ServiceConstants.UNKNOWN;
                }
                if (!string.IsNullOrEmpty(person.gender))
                {
                    personToBeAddedToDatabase.Gender = person.gender;
                }
                else
                {
                    personToBeAddedToDatabase.Gender = ServiceConstants.UNKNOWN;
                }
                if (person.status != null)
                {
                    if (!string.IsNullOrEmpty(person.status.name))
                    {
                        personToBeAddedToDatabase.Status = person.status.name;
                    }
                    else
                    {
                        person.status.name = ServiceConstants.UNKNOWN;
                    }
                    if (person.status.date != null)
                    {
                        personToBeAddedToDatabase.StatusDate = (DateTime)person.status.date;
                    }
                    if (!string.IsNullOrEmpty(person.status.comment))
                    {
                        personToBeAddedToDatabase.StatusComment = person.status.comment;
                    }
                    if (person.status.subStatus != null && !string.IsNullOrEmpty(person.status.subStatus.name))
                    {
                        personToBeAddedToDatabase.Substatus = person.status.subStatus.name;
                    }
                    else
                    {
                        personToBeAddedToDatabase.Substatus = ServiceConstants.NONE;
                    }
                }
                else
                {
                    personToBeAddedToDatabase.Status = ServiceConstants.UNKNOWN;
                    personToBeAddedToDatabase.Substatus = ServiceConstants.NONE;
                }
                if (!string.IsNullOrEmpty(person.employer))
                {
                    personToBeAddedToDatabase.Employer = person.employer;
                }
                if (person.occupation != null)
                {
                    if (!string.IsNullOrEmpty(person.occupation.name))
                    {
                        personToBeAddedToDatabase.OccupationName = person.occupation.name;
                    }
                    if (!string.IsNullOrEmpty(person.occupation.description))
                    {
                        personToBeAddedToDatabase.OccupationDescription = person.occupation.description;
                    }
                }
                if (!string.IsNullOrEmpty(person.firstName))
                {
                    personToBeAddedToDatabase.FirstName = person.firstName;
                }
                if (!string.IsNullOrEmpty(person.middleName))
                {
                    personToBeAddedToDatabase.MiddleName = person.middleName;
                }
                if (!string.IsNullOrEmpty(person.lastName))
                {
                    personToBeAddedToDatabase.LastName = person.lastName;
                }
                if (!string.IsNullOrEmpty(person.formerName))
                {
                    personToBeAddedToDatabase.FormerName = person.formerName;
                }
                if (!string.IsNullOrEmpty(person.formerChurch))
                {
                    personToBeAddedToDatabase.FormerChurch = person.formerChurch;
                }
                if (!string.IsNullOrEmpty(person.goesByName))
                {
                    personToBeAddedToDatabase.GoesByName = person.goesByName;
                }
                if (person.lastMatchDate !=null)
                {
                    personToBeAddedToDatabase.LastMatchDate = (DateTime)person.lastMatchDate;
                }
                if (person.lastUpdatedDate != null)
                {
                    personToBeAddedToDatabase.LastUpdatedDate = (DateTime)person.lastUpdatedDate;
                }
                if (!string.IsNullOrEmpty(person.maritalStatus))
                {
                    personToBeAddedToDatabase.MaritalStatus = person.maritalStatus;
                }
                else
                {
                    personToBeAddedToDatabase.MaritalStatus = ServiceConstants.UNKNOWN;
                }
                if (!string.IsNullOrEmpty(person.salutation))
                {
                    personToBeAddedToDatabase.Salutation = person.salutation;
                } 
                if (person.school != null && !string.IsNullOrEmpty(person.school.name))
                {
                    personToBeAddedToDatabase.School = person.school.name;
                }
                if (!string.IsNullOrEmpty(person.suffix))
                {
                    personToBeAddedToDatabase.Suffix = person.suffix;
                }
                if (!string.IsNullOrEmpty(person.title))
                {
                    personToBeAddedToDatabase.Title = person.title;
                }
                if (!string.IsNullOrEmpty(person.imageFilePath))
                {
                    personToBeAddedToDatabase.ImageFilePath = person.imageFilePath;
                }
                if (person.addresses != null && person.addresses.address != null)
                {
                    updatePersonWithAddresses(personToBeAddedToDatabase, person.addresses.address);
                }
                if (person.attributes != null && person.attributes.attribute != null)
                {
                    updatePersonWithEvents(personToBeAddedToDatabase, person.attributes.attribute);
                }
                if (person.communications != null && person.communications.communication!= null)
                {
                    updatePersonWithCommunications(personToBeAddedToDatabase, person.communications.communication);
                }
                if (person.peopleRequirements != null && person.peopleRequirements.peopleRequirement != null)
                {
                    updatePersonWithBackgroundChecks(personToBeAddedToDatabase, person.peopleRequirements.peopleRequirement);
                }
                personnelToBeAddedToDatabase.Add(personToBeAddedToDatabase);
            }
            _northcrestLocalData.RefreshNorthcrestDatabase(personnelToBeAddedToDatabase);
        }

        private void updatePersonWithAddresses(
            Northcrest.Domain.FellowshipOne.Person person,
            PersonAddress[] addresses)
        {
            if (addresses != null)
            {
                foreach (PersonAddress address in addresses)
                {
                    Northcrest.Domain.FellowshipOne.Address addressToBeAddedToDatabase = new Northcrest.Domain.FellowshipOne.Address();
                    if (!string.IsNullOrEmpty(address.address1))
                    {
                        addressToBeAddedToDatabase.Address1 = address.address1;
                    }
                    if (!string.IsNullOrEmpty(address.address2))
                    {
                        addressToBeAddedToDatabase.Address2 = address.address2;
                    }
                    if (!string.IsNullOrEmpty(address.address3))
                    {
                        addressToBeAddedToDatabase.Address3 = address.address3;
                    }
                    if (!string.IsNullOrEmpty(address.city))
                    {
                        addressToBeAddedToDatabase.City = address.city;
                    }
                    if (!string.IsNullOrEmpty(address.stProvince))
                    {
                        addressToBeAddedToDatabase.State = address.stProvince;
                    }
                    if (!string.IsNullOrEmpty(address.county))
                    {
                        addressToBeAddedToDatabase.County = address.country;
                    }
                    if (!string.IsNullOrEmpty(address.country))
                    {
                        addressToBeAddedToDatabase.Country = address.country;
                    }
                    if (!string.IsNullOrEmpty(address.postalCode))
                    {
                        addressToBeAddedToDatabase.PostalCode = address.postalCode;
                    }
                    if (address.addressType != null && !string.IsNullOrEmpty(address.addressType.name))
                    {
                        addressToBeAddedToDatabase.Type = address.addressType.name;
                    }
                    if (address.addressDate != null)
                    {
                        addressToBeAddedToDatabase.AddressDate = (DateTime)address.addressDate;
                    }
                    person.Addresses.Add(addressToBeAddedToDatabase);
                }
            }
        }

        private void updatePersonWithEvents(
            Northcrest.Domain.FellowshipOne.Person person,
            PersonAttribute[] events)
        {
            if (events != null)
            {
                foreach (PersonAttribute personEvent in events)
                {
                    Northcrest.Domain.FellowshipOne.Event eventToBeAddedToDatabase = new Northcrest.Domain.FellowshipOne.Event();
                    if(personEvent.attributeGroup != null &&
                        personEvent.attributeGroup.attribute != null && 
                        !string.IsNullOrEmpty(personEvent.attributeGroup.attribute.name))
                    {
                        eventToBeAddedToDatabase.Name = personEvent.attributeGroup.attribute.name;
                    }
                    else
                    {
                        eventToBeAddedToDatabase.Name = ServiceConstants.UNKNOWN;
                    }
                    if (!string.IsNullOrEmpty(personEvent.comment))
                    {
                        eventToBeAddedToDatabase.Comment = personEvent.comment;
                    }
                    if (personEvent.createdDate != null)
                    {
                        eventToBeAddedToDatabase.CreatedDate = (DateTime)personEvent.createdDate;
                    }
                    if (personEvent.startDate != null)
                    {
                        eventToBeAddedToDatabase.StartDate = (DateTime)personEvent.startDate;
                    }
                    if (personEvent.endDate != null)
                    {
                        eventToBeAddedToDatabase.EndDate = (DateTime)personEvent.endDate;
                    }
                    if (personEvent.lastUpdateddDate != null)
                    {
                        eventToBeAddedToDatabase.LastUpdatedDate = (DateTime)personEvent.lastUpdateddDate;
                    }
                    person.Events.Add(eventToBeAddedToDatabase);
                }
            }
        }

        private void updatePersonWithCommunications(
            Northcrest.Domain.FellowshipOne.Person person,
            PersonCommunication[] communications)
        {
            if (communications != null)
            {
                foreach (PersonCommunication communication in communications)
                {
                    Northcrest.Domain.FellowshipOne.Communication communicationToBeAddedToDatabase = new Northcrest.Domain.FellowshipOne.Communication();
                    if (communication.communicationType != null && !string.IsNullOrEmpty(communication.communicationType.name))
                    {
                        communicationToBeAddedToDatabase.SpecificType = communication.communicationType.name;
                    }
                    else
                    {
                        communicationToBeAddedToDatabase.SpecificType = ServiceConstants.UNKNOWN;
                    }
                    if (!string.IsNullOrEmpty(communication.communicationComment))
                    {
                        communicationToBeAddedToDatabase.Comment = communication.communicationComment;
                    }
                    if (!string.IsNullOrEmpty(communication.communicationGeneralType))
                    {
                        communicationToBeAddedToDatabase.GeneralType = communication.communicationGeneralType;
                    }
                    if (!string.IsNullOrEmpty(communication.communicationValue))
                    {
                        communicationToBeAddedToDatabase.Value = communication.communicationValue;
                    }
                    if (communication.preferred != null)
                    {
                        communicationToBeAddedToDatabase.Preferred = (Boolean)communication.preferred;
                    }
                    if (!string.IsNullOrEmpty(communication.searchCommunicationValue))
                    {
                        communicationToBeAddedToDatabase.SearchValue = communication.searchCommunicationValue;
                    }


                    person.Communications.Add(communicationToBeAddedToDatabase);
                }
            }
        }

        private void updatePersonWithBackgroundChecks(
            Northcrest.Domain.FellowshipOne.Person person,
            PersonRequirement[] backgroundChecks
            )
        {
            if (backgroundChecks != null)
            {
                foreach (PersonRequirement check in backgroundChecks)
                {
                    Northcrest.Domain.FellowshipOne.BackgroundCheck checksToBeAddedToDatabase = new Northcrest.Domain.FellowshipOne.BackgroundCheck();
                    if (check.backgroundCheck != null)
                    {
                        if (check.backgroundCheck.trackingNumber != null)
                        {
                            checksToBeAddedToDatabase.TrackingNumber = (int)check.backgroundCheck.trackingNumber;
                        }
                        else
                        {
                            checksToBeAddedToDatabase.TrackingNumber = -1;
                        }
                        if (check.backgroundCheck.requestDate != null)
                        {
                            checksToBeAddedToDatabase.RequestDate = (DateTime)check.backgroundCheck.requestDate;
                        }
                        if (check.backgroundCheck.backgroundCheckStatus != null)
                        {
                            if (!string.IsNullOrEmpty(check.backgroundCheck.backgroundCheckStatus.name))
                            {
                                checksToBeAddedToDatabase.Status = check.backgroundCheck.backgroundCheckStatus.name;
                            }
                            else
                            {
                                checksToBeAddedToDatabase.Status = ServiceConstants.NO_STATUS_PROVIDED;
                            }
                        }
                        else
                        {
                            checksToBeAddedToDatabase.Status = ServiceConstants.NO_STATUS_PROVIDED;
                        }
                    }
                    person.BackgroundChecks.Add(checksToBeAddedToDatabase);
                }
            }
        }

    }
}
