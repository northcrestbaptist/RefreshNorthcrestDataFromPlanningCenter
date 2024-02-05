using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromFellowshipOne.Common.Constants;
using RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne;
using RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Services
{ 
    public class RetrieveFellowshipOneDataService : IRetrieveFellowshipOneDataService
    {
        private readonly ILogger<RetrieveFellowshipOneDataService> _log;
        private readonly INorthcrestConfigurationService _northcrestConfigurationService;
        //private readonly IDatabaseUpdateService _databaseUpdateService;
        //private readonly IRetrievePlanDataService _retrievePlanDataService;
        private FellowshipOneInformation _fellowshipOneInfo;

        public RetrieveFellowshipOneDataService(
            ILogger<RetrieveFellowshipOneDataService> log,
            INorthcrestConfigurationService northcrestConfigurationService
            //IDatabaseUpdateService databaseUpdateService,
            //IRetrievePlanDataService retrievePlanDataService
            //
            )
        {
            _log = log;
            _northcrestConfigurationService = northcrestConfigurationService;
            //_databaseUpdateService = databaseUpdateService;
            //_retrievePlanDataService = retrievePlanDataService;
            _fellowshipOneInfo = new FellowshipOneInformation();
        }
        public async Task RunAsync()
        {
            _log.LogInformation("Starting personnel data refresh...");
            using (_fellowshipOneInfo.Client = new())
            {
                try
                {
                    _northcrestConfigurationService.LogLocalAppConfigurationInstructions();
                    _log.LogInformation("Retrieving and setting Northcrest and Fellowship One configurations...");
                    _northcrestConfigurationService.SetConfiguration(_fellowshipOneInfo);
                    bool refreshSomething = _northcrestConfigurationService.IsAnythingConfiguredToRefresh(_fellowshipOneInfo);
                    if (refreshSomething)
                    {
                        //_databaseUpdateService.DeleteLocalDataToBeRefreshed(_availablePlansInfo);
                        _log.LogInformation("Retrieving data from Fellowship One according to current configurations...");
                        _log.LogInformation("Starting data pull for main personnel records for all Northcrest Baptist Church personnel...");
                        await GetMainPersonDataFromFellowshipOneAsync();
                        _log.LogInformation("Completed data pull for main personnel records.");
                        _log.LogInformation("Starting data pull for backgound investigation information for each person of Northcrest Baptist Church...");
                        await GetImageForEachPersonWithAnImageUri();
                        //await GetBackgroundInvestigationDataForEachPersonFromFellowshipOneAsync();
                        _log.LogInformation("Completed data pull for each person's background investigation information.");


                        //_databaseUpdateService.RefreshDataThatWasRetrievedFromPlanningCenter(_availablePlansInfo);
                    }
                    else
                    {
                        _log.LogInformation("None of the data is configured to refresh.");
                        //_northcrestConfigurationService.LogLocalAppConfigurationInstructions();
                    }
                }
                catch(Exception ex)
                {
                    _log.LogInformation("Encountered the following error: {error}", ex.Message);
                    _log.LogInformation("Error source: {source}", ex.Source);
                    _log.LogInformation("Stack Trace of error: {stackTrace}", ex.StackTrace);
                }

            }
            //_log.LogInformation("A total of {genSongsCount} songs were refreshed to the GeneralSongs table during a search through {plansCount} service plans.",
            //    _availablePlansInfo.NumberOfGeneralSongsRefreshed, _availablePlansInfo.NumberofPlansUsedToRefreshGeneralSongs);
            _log.LogInformation("Date refresh completed successfully.");
            
        }

        private async Task GetMainPersonDataFromFellowshipOneAsync()
        {
            var responseTask = _fellowshipOneInfo.Client.GetAsync(_fellowshipOneInfo.GetAllPeopleApi);
            responseTask.Wait();

            var result = responseTask.Result;
            var readReturnedDataTask = result.Content.ReadAsStringAsync();
            readReturnedDataTask.Wait();
            string personDataPullResults = readReturnedDataTask.Result;
                       
            _fellowshipOneInfo.fellowshipOnePersonDataPull = JsonConvert.DeserializeObject<FellowshipOnePersonDataPull>(personDataPullResults);
            _fellowshipOneInfo.TotalRecordsPulled = _fellowshipOneInfo.fellowshipOnePersonDataPull.results.totalRecords;

            _log.LogInformation("Total Count of dowloaded Personnel: {count}", _fellowshipOneInfo.fellowshipOnePersonDataPull.results.totalRecords);
            
        }

        private async Task GetImageForEachPersonWithAnImageUri()
        {
            // Delete all files in a directory    
            string[] files = Directory.GetFiles(ServiceConstants.FILE_PATH_PERSON_IMAGE);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            int imageCount = 0;
            foreach(Person person in _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person)
            {
                if(!String.IsNullOrEmpty(person.imageURI))
                {
                    imageCount++;
                    _log.LogInformation("Pulling image for: {name}", $"{person.lastName}, {person.firstName}");
                    byte[] byteArray;
                    string fileExtension;
                    var actualFileResponseTask = _fellowshipOneInfo.Client.GetStreamAsync(person.imageURI);
                    actualFileResponseTask.Wait();
                    MemoryStream ms = new MemoryStream();
                    

                    await actualFileResponseTask.Result.CopyToAsync(ms);
                    byteArray = ms.ToArray();  
                    Image image = Image.FromStream(ms);

                    _log.LogInformation("Rawformat: {format}", image.RawFormat.ToString());
                    if (ImageFormat.Png.Equals(image.RawFormat))
                    {
                        fileExtension = "png";
                    }
                    else if (ImageFormat.Jpeg.Equals(image.RawFormat))
                    {
                        fileExtension = "jpeg";
                    }
                    else if (ImageFormat.Bmp.Equals(image.RawFormat))
                    {
                        fileExtension = "bmp";
                    }
                    else if (ImageFormat.Gif.Equals(image.RawFormat))
                    {
                        fileExtension = "gif";
                    }
                    else
                    {
                        fileExtension = "jpeg";
                    }
                    person.imageFilePath = $"{ServiceConstants.FILE_PATH_PERSON_IMAGE + person.lastName + person.firstName + person.id}.{fileExtension}";
                    await File.WriteAllBytesAsync(person.imageFilePath, byteArray);
                }
            }
            _log.LogInformation("Pulled and saved {numberOfImages} images.", imageCount);
        }

        private async Task GetBackgroundInvestigationDataForEachPersonFromFellowshipOneAsync()
        {
            for(int i = 0; i < _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person.Length; i++)
            {
                _log.LogInformation("Pulling bacgound check data for record {count} of {total} personnel...", i + 1, _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person.Length);
                _fellowshipOneInfo.GetPersonBackgroundInvestigationInfoApi =
                    _fellowshipOneInfo.GetPersonBackgroundInvestigationInfoApiTemplate.Replace("#", _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person[i].id.ToString());
                var responseTask = _fellowshipOneInfo.Client.GetAsync(_fellowshipOneInfo.GetPersonBackgroundInvestigationInfoApi);
                responseTask.Wait();

                var result = responseTask.Result;
                var readReturnedDataTask = result.Content.ReadAsStringAsync();
                readReturnedDataTask.Wait();
                string personBackGroundInvestiationDataPullResults = readReturnedDataTask.Result;
                FellowshipOneRequirementDataPull f1RequirementsDataForAllPersonnel = JsonConvert.DeserializeObject<FellowshipOneRequirementDataPull>(personBackGroundInvestiationDataPullResults);
                
                if (f1RequirementsDataForAllPersonnel != null
                    && f1RequirementsDataForAllPersonnel.peopleRequirements != null
                    && f1RequirementsDataForAllPersonnel.peopleRequirements.peopleRequirement != null
                   )
                {
                    _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person[i].peopleRequirements = f1RequirementsDataForAllPersonnel.peopleRequirements;
                }
                else if (personBackGroundInvestiationDataPullResults != "{\"peopleRequirements\":null}")
                {
                    _log.LogInformation("get security results but no object created. {result}", personBackGroundInvestiationDataPullResults);
                }

            }

            _log.LogInformation("Background check information downloaded for all personnel.");
            _log.LogInformation("Now checking each person's record for background check date and displaying is present...");
            int totalCountOfPersonsWithRequirements = 0;
            foreach (Person person in _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person)
            {
                
                if (person.peopleRequirements != null && person.peopleRequirements.peopleRequirement != null)
                {
                    _log.LogInformation("----------------------------------------------");
                    _log.LogInformation("----------------------------------------------");
                    _log.LogInformation("---------------PERSON DIVIDER-----------------");
                    _log.LogInformation("----------------------------------------------");
                    _log.LogInformation("----------------------------------------------");
                    _log.LogInformation("Name: {result}", person.lastName + ", " + person.firstName);
                    _log.LogInformation("Household number: {result}", person.householdID);
                    int rqmtCount = 0;
                    totalCountOfPersonsWithRequirements++;
                    foreach (PersonRequirement rqmt in person.peopleRequirements.peopleRequirement)
                    {
                        rqmtCount++;
                        _log.LogInformation("Requirement #: {result}", rqmtCount);
                        if (rqmt.id != null)
                        {
                            _log.LogInformation("Requirement ID: {result}", rqmt.id);
                        }
                        if (rqmt.lastUpdatedDate != null)
                        {
                            _log.LogInformation("Requirment Last Updated Date: {result}", !String.IsNullOrEmpty(rqmt.lastUpdatedDate.ToString()) ? rqmt.lastUpdatedDate.ToString() : "Not available");
                        }
                        else
                        {
                            _log.LogInformation("Requirement Last Updated Date: {result}", "Not available");
                        }

                        if (rqmt.createdDate != null)
                        {
                            _log.LogInformation("Requirement Created Date: {result}", !String.IsNullOrEmpty(rqmt.createdDate.ToString()) ? rqmt.createdDate.ToString() : "Not available");
                        }
                        else
                        {
                            _log.LogInformation("Requirement Created Date: {result}", "Not available");
                        }
                        if (rqmt.staffPerson != null)
                        {
                            _log.LogInformation("Has Staff Person ID: {result}", !String.IsNullOrEmpty(rqmt.staffPerson.id.ToString()) ? rqmt.staffPerson.id : "Not available");
                        }
                        else
                        {
                            _log.LogInformation("Has no Staff Person ID.");
                        }
                        if (rqmt.requirement != null)
                        {
                            _log.LogInformation("Requirement Name: {result}", !String.IsNullOrEmpty(rqmt.requirement.name) ? rqmt.requirement.name : "Not available");
                        }
                        else
                        {
                            _log.LogInformation("Requirement Name: {result}", "Not available");
                        }
                        if (rqmt.requirementDate != null)
                        {
                            _log.LogInformation("Requirement Date: {result}", !String.IsNullOrEmpty(rqmt.requirementDate.ToString()) ? rqmt.requirementDate.ToString() : "Not available");
                        }
                        else
                        {
                            _log.LogInformation("Requirement Date: {result}", "Not available");
                        }
                        if (rqmt.requirementDocumentUri != null)
                        {
                            _log.LogInformation("Requirement Document URI: {result}", !String.IsNullOrEmpty(rqmt.requirementDocumentUri) ? rqmt.requirementDocumentUri : "Not available");
                        }
                        else
                        {
                            _log.LogInformation("Requirement Document URI: {result}", "Not available");
                        }
                        if (rqmt.requirementStatus != null)
                        {
                            _log.LogInformation("Requirement Status: {result}", !String.IsNullOrEmpty(rqmt.requirementStatus.name) ? rqmt.requirementStatus.name : "Not available");
                        }
                        else
                        {
                            _log.LogInformation("Requirement Status: {result}", "Not available");
                        }
                        if (rqmt.backgroundCheck != null)
                        {
                            if (rqmt.backgroundCheck.backgroundCheckStatus != null)
                            {
                                _log.LogInformation("Backgound Check Status: {result}", !String.IsNullOrEmpty(rqmt.backgroundCheck.backgroundCheckStatus.name) ? rqmt.backgroundCheck.backgroundCheckStatus.name : "Not available");
                            }
                            else
                            {
                                _log.LogInformation("Requirement Check Status: {result}", "Not available");
                            }
                            if (rqmt.backgroundCheck.trackingNumber != null)
                            {
                                _log.LogInformation("Backgound Check Tracking Number: {result}", !String.IsNullOrEmpty(rqmt.backgroundCheck.trackingNumber.ToString()) ? rqmt.backgroundCheck.trackingNumber : "Not available");
                            }
                            else
                            {
                                _log.LogInformation("Backgound Check Tracking Number: {result}", "Not available");
                            }
                            if (rqmt.backgroundCheck.requestDate != null)
                            {
                                _log.LogInformation("Backgound Check Request Date: {result}", !String.IsNullOrEmpty(rqmt.backgroundCheck.requestDate.ToString()) ? rqmt.backgroundCheck.requestDate : "Not available");
                            }
                            else
                            {
                                _log.LogInformation("Background Check Request Date: {result}", "Not available");
                            }
                        }
                        else
                        {
                            _log.LogInformation("Requirement Statu: {result}", "Not available");
                        }
                    }
                    _log.LogInformation("This person had {result} requirement records", rqmtCount);
                }

            }

            _log.LogInformation("There were {result} personnel in Northcrest with requirement records, which is where background check data is stored.", totalCountOfPersonsWithRequirements);
        }
    }
    //foreach (Person person in _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person)
    //{
    //    _log.LogInformation("Name: {result}", person.lastName + ", " + person.firstName);
    //    _log.LogInformation("Household number: {result}", person.householdID);
    //    if (person.householdMemberType != null)
    //    {
    //        _log.LogInformation("Household member type Type: {result}", person.householdMemberType.name);
    //    }
    //    else
    //    {
    //        _log.LogInformation("Household member type: {result}", "Not available");
    //    }
    //    if (person.status != null)
    //    {
    //        _log.LogInformation("Membership Status: {result}", person.status.name);
    //        _log.LogInformation("Status Comment: {result}", !String.IsNullOrEmpty(person.status.comment) ? person.status.comment : "No comment.");
    //        if(person.status.subStatus != null)
    //        {
    //            _log.LogInformation("Membership Substatus: {result}", !String.IsNullOrEmpty(person.status.subStatus.name) ? person.status.subStatus.name : "No substatus.");
    //        }
    //    }
    //    else
    //    {
    //        _log.LogInformation("Membership Status: {result}", "Not available");
    //    }
    //    _log.LogInformation("Gender: {result}", !String.IsNullOrEmpty(person.gender) ? person.gender : "Not available");
    //    _log.LogInformation("Marital Status: {result}", !String.IsNullOrEmpty(person.maritalStatus) ? person.maritalStatus : "Not available");
    //    _log.LogInformation("Employer: {result}", !String.IsNullOrEmpty(person.employer) ? person.employer : "Not available");
    //    _log.LogInformation("Former Church: {result}", !String.IsNullOrEmpty(person.formerChurch) ? person.formerChurch : "Not available");
    //    if(person.dateOfBirth != null)
    //    {
    //        _log.LogInformation("Date of Birth: {result}", !String.IsNullOrEmpty(person.dateOfBirth.ToString()) ? person.dateOfBirth.ToString() : "Not available");
    //    }
    //    else
    //    {
    //        _log.LogInformation("Date of Birth: {result}", "Not available");
    //    }
    //    if (person.occupation != null)
    //    {
    //        _log.LogInformation("Occupation Name: {result}", !String.IsNullOrEmpty(person.occupation.name) ? person.occupation.name : "Not available");
    //        _log.LogInformation("Occupation Description: {result}", !String.IsNullOrEmpty(person.occupation.description) ? person.occupation.description : "Not available");
    //    }
    //    else
    //    {
    //        _log.LogInformation("Occupation: {result}", "Not available");
    //    }
    //    if (person.school != null)
    //    {
    //        _log.LogInformation("School Name: {result}", !String.IsNullOrEmpty(person.school.name) ? person.school.name : "Not available");
    //    }
    //    else
    //    {
    //        _log.LogInformation("School Name: {result}", "Not available");
    //    }
    //    if (person.denomination != null)
    //    {
    //        _log.LogInformation("Denomination: {result}", !String.IsNullOrEmpty(person.denomination.name) ? person.denomination.name : "Not available");
    //    }
    //    else
    //    {
    //        _log.LogInformation("Denomination: {result}", "Not available");
    //    }
    //    if (person.attributes != null && person.attributes.attribute != null)
    //    {
    //        int attCount = 0;
    //        foreach(PersonAttribute att in person.attributes.attribute)
    //        {
    //            attCount++;
    //            _log.LogInformation("Attribute: {result}", attCount);
    //            if (att.attributeGroup != null) {
    //                _log.LogInformation("Att Group: {result}", !String.IsNullOrEmpty(att.attributeGroup.name) ? att.attributeGroup.name : "Not available");
    //                if(att.attributeGroup.attribute != null)
    //                {
    //                    _log.LogInformation("Att Name: {result}", !String.IsNullOrEmpty(att.attributeGroup.attribute.name) ? att.attributeGroup.attribute.name : "Not available");
    //                }
    //            }
    //            else
    //            {
    //                _log.LogInformation("Att Group: {result}", "Not available");
    //            }
    //            if (att.createdDate != null)
    //            {
    //                _log.LogInformation("Att Created Date: {result}", !String.IsNullOrEmpty(att.createdDate.ToString()) ? att.createdDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Att Created Date: {result}", "Not available");
    //            }
    //            if (att.startDate != null)
    //            {
    //                _log.LogInformation("Att Start Date: {result}", !String.IsNullOrEmpty(att.startDate.ToString()) ? att.startDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Att Start Date: {result}", "Not available");
    //            }
    //            if (att.endDate != null)
    //            {
    //                _log.LogInformation("Att End Date: {result}", !String.IsNullOrEmpty(att.endDate.ToString()) ? att.endDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Att End Date: {result}", "Not available");
    //            }
    //            _log.LogInformation("Att Comment: {result}", !String.IsNullOrEmpty(att.comment) ? att.comment : "Not available");
    //        }
    //    }
    //    else
    //    {
    //        _log.LogInformation("Attributes: {result}", "Not available");
    //    }
    //    if (person.addresses != null && person.addresses.address != null)
    //    {
    //        int addCount = 0;
    //        foreach (PersonAddress add in person.addresses.address)
    //        {
    //            addCount++;
    //            _log.LogInformation("Address: {result}", addCount);
    //            if (add.addressType != null)
    //            {
    //                _log.LogInformation("Address Type: {result}", !String.IsNullOrEmpty(add.addressType.name) ? add.addressType.name : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Address Type: {result}", "Not available");
    //            }
    //            if (add.createdDate != null)
    //            {
    //                _log.LogInformation("Address Created Date: {result}", !String.IsNullOrEmpty(add.createdDate.ToString()) ? add.createdDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Address Created Date: {result}", "Not available");
    //            }
    //            if (add.addressDate != null)
    //            {
    //                _log.LogInformation("Address Date: {result}", !String.IsNullOrEmpty(add.addressDate.ToString()) ? add.addressDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("AddressDate: {result}", "Not available");
    //            }
    //            if (add.lastUpdatedDate != null)
    //            {
    //                _log.LogInformation("Address Last Updated Date: {result}", !String.IsNullOrEmpty(add.lastUpdatedDate.ToString()) ? add.lastUpdatedDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Address Last Updated Date: {result}", "Not available");
    //            }
    //            _log.LogInformation("Address1: {result}", !String.IsNullOrEmpty(add.address1) ? add.address1 : "");
    //            _log.LogInformation("Address2: {result}", !String.IsNullOrEmpty(add.address2) ? add.address2 : "");
    //            _log.LogInformation("Address3: {result}", !String.IsNullOrEmpty(add.address3) ? add.address3 : "");
    //            _log.LogInformation("{city}, {state} {zipcode}", 
    //                !String.IsNullOrEmpty(add.city) ? add.city : "",
    //                !String.IsNullOrEmpty(add.stProvince) ? add.stProvince : "",
    //                !String.IsNullOrEmpty(add.postalCode) ? add.postalCode : "");
    //            _log.LogInformation("County: {result}", !String.IsNullOrEmpty(add.county) ? add.county : "Not available");
    //            _log.LogInformation("Country: {result}", !String.IsNullOrEmpty(add.country) ? add.country : "Not available");
    //        }
    //    }
    //    else
    //    {
    //        _log.LogInformation("Addresses: {result}", "Not available");
    //    }
    //    if (person.communications != null && person.communications.communication != null)
    //    {
    //        int commCount = 0;
    //        foreach (PersonCommunication comm in person.communications.communication)
    //        {
    //            commCount++;
    //            _log.LogInformation("Communication: {result}", commCount);
    //            if (comm.communicationType != null)
    //            {
    //                _log.LogInformation("Communication Type: {result}", !String.IsNullOrEmpty(comm.communicationType.name) ? comm.communicationType.name : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Communication Type: {result}", "Not available");
    //            }
    //            if (comm.createdDate != null)
    //            {
    //                _log.LogInformation("Communication Created Date: {result}", !String.IsNullOrEmpty(comm.createdDate.ToString()) ? comm.createdDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Communication Created Date: {result}", "Not available");
    //            }
    //            if (comm.lastUpdatedDate != null)
    //            {
    //                _log.LogInformation("Communication Last Updated Date: {result}", !String.IsNullOrEmpty(comm.lastUpdatedDate.ToString()) ? comm.lastUpdatedDate.ToString() : "Not available");
    //            }
    //            else
    //            {
    //                _log.LogInformation("Communication Last Updated Date: {result}", "Not available");
    //            }
    //            _log.LogInformation("Communication General Type: {result}", !String.IsNullOrEmpty(comm.communicationGeneralType) ? comm.communicationGeneralType : "Not available");
    //            _log.LogInformation("Communication Value: {result}", !String.IsNullOrEmpty(comm.communicationValue) ? comm.communicationValue : "Not available");
    //            _log.LogInformation("Address3: {result}", !String.IsNullOrEmpty(comm.communicationComment) ? comm.communicationComment : "No comment");
    //            if (comm.preferred != null)
    //            {
    //                _log.LogInformation("Preferred: {result}", comm.preferred);
    //            }
    //            else
    //            {
    //                _log.LogInformation("Preferred: {result}", "Not available");
    //            }

    //        }
    //    }
    //    else
    //    {
    //        _log.LogInformation("Communications: {result}", "Not available");
    //    }
    //    if (person.createdDate != null)
    //    {
    //        _log.LogInformation("Person Created Date: {result}", !String.IsNullOrEmpty(person.createdDate.ToString()) ? person.createdDate.ToString() : "Not available");
    //    }
    //    else
    //    {
    //        _log.LogInformation("Person Created Date: {result}", "Not available");
    //    }
    //    if (person.lastUpdatedDate != null)
    //    {
    //        _log.LogInformation("Person Last Updated Date: {result}", !String.IsNullOrEmpty(person.lastUpdatedDate.ToString()) ? person.lastUpdatedDate.ToString() : "Not available");
    //    }
    //    else
    //    {
    //        _log.LogInformation("Person Last Updated Date: {result}", "Not available");
    //    }
    //}
}
