using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RefreshNorthcrestDataFromFellowshipOne.Common.Constants;
using RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne;
using RefreshNorthcrestDataFromFellowshipOne.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace RefreshNorthcrestDataFromFellowshipOne.Services
{ 
    public class RetrieveFellowshipOneDataService : IRetrieveFellowshipOneDataService
    {
        private readonly ILogger<RetrieveFellowshipOneDataService> _log;
        private readonly INorthcrestConfigurationService _northcrestConfigurationService;
        private readonly IDatabaseUpdateService _databaseUpdateService;
        private FellowshipOneInformation _fellowshipOneInfo;

        public RetrieveFellowshipOneDataService(
            ILogger<RetrieveFellowshipOneDataService> log,
            INorthcrestConfigurationService northcrestConfigurationService,
            IDatabaseUpdateService databaseUpdateService
            )
        {
            _log = log;
            _northcrestConfigurationService = northcrestConfigurationService;
            _databaseUpdateService = databaseUpdateService;
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
                        _log.LogInformation("Retrieving data from Fellowship One according to current configurations...");
                        _log.LogInformation("Starting data pull for main personnel records for all Northcrest Baptist Church personnel...");
                        await GetMainPersonDataFromFellowshipOneAsync();
                        _log.LogInformation("Completed data pull for main personnel records.");
                        _log.LogInformation("Starting data pull for profile images for those personnel who have images on file for all Northcrest Baptist Church...");
                        await GetImageForEachPersonWithAnImageUri();
                        _log.LogInformation("Starting data pull for backgound investigation information for each person of Northcrest Baptist Church...");
                        await GetBackgroundInvestigationDataForEachPersonFromFellowshipOneAsync();
                        _log.LogInformation("Completed data pull for each person's background investigation information.");
                        _databaseUpdateService.RefreshDataThatWasRetrievedFromFellowshipOne(_fellowshipOneInfo);
                        
                    }
                    else
                    {
                        _log.LogInformation("None of the data is configured to refresh.");
                        _northcrestConfigurationService.LogLocalAppConfigurationInstructions();
                    }
                }
                catch(Exception ex)
                {
                    _log.LogInformation("Date refresh failed.");
                    _log.LogInformation("Encountered the following error: {error}", ex.Message);
                    _log.LogInformation("Error source: {source}", ex.Source);
                    _log.LogInformation("Stack Trace of error: {stackTrace}", ex.StackTrace);
                }
            }
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
                    byte[] byteArray;
                    string fileExtension;
                    var actualFileResponseTask = _fellowshipOneInfo.Client.GetStreamAsync(person.imageURI);
                    actualFileResponseTask.Wait();
                    MemoryStream ms = new MemoryStream();
                    

                    await actualFileResponseTask.Result.CopyToAsync(ms);
                    byteArray = ms.ToArray();  
                    Image image = Image.FromStream(ms);

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
                    //_log.LogInformation("get security results but no object created. {result}", personBackGroundInvestiationDataPullResults);
                }

            }

            _log.LogInformation("Background check information downloaded for all personnel.");
            int totalCountOfPersonsWithRequirements = 0;
            foreach (Person person in _fellowshipOneInfo.fellowshipOnePersonDataPull.results.person)
            {
                if (person.peopleRequirements != null && person.peopleRequirements.peopleRequirement != null)
                {
                    totalCountOfPersonsWithRequirements++;
                }
            }

            _log.LogInformation("There were {result} personnel in Northcrest with requirement records, which is where background check data is stored.", totalCountOfPersonsWithRequirements);
        }
    }

}
