using RefreshNorthcrestDataFromFellowshipOne.Models.Local;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class FellowshipOneInformation
    {
        public HttpClient? Client { get; set; }
        public LocalAppConfiguration? RefreshAppConfig { get; set; }
        public FellowshipOneConfiguration? FellowshipOneConfig { get; set; }
        public string? BaseUrl { get; set; } = string.Empty;
        public string? GetAllPeopleApi { get; set; } = string.Empty;
        public string? GetPersonBackgroundInvestigationInfoApiTemplate { get; set; }
        public string? GetPersonBackgroundInvestigationInfoApi { get; set; }
        public int? TotalRecordsPulled { get; set; }
        public FellowshipOnePersonDataPull? fellowshipOnePersonDataPull { get; set; }
        public IList<Northcrest.Domain.FellowshipOne.Person> Persons { get; set; } = new List<Northcrest.Domain.FellowshipOne.Person>();

    }
}
