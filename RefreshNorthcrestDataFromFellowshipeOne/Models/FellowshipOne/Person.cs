using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    // prop short cut prop tab tab int tab Id tab
    public class Person
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        [JsonProperty("@uri")]
        public string? uri { get; set; }
        [JsonProperty("@imageURI")]
        public string? imageURI { get; set; }
        public string? imageFilePath { get; set; }
        [JsonProperty("@oldID")]
        public int? oldID { get; set; }
        [JsonProperty("@iCode")]
        public string? iCode { get; set; }
        [JsonProperty("@householdID")]
        public int? householdID { get; set; }
        [JsonProperty("@oldHousholdID")]
        public int? oldHousholdID { get; set; }
        public string? title { get; set; }
        public string? salutation { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? suffix { get; set; }
        public string? middleName { get; set; }
        public string? goesByName { get; set; }
        public string? formerName { get; set; }
        public string? gender { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string? maritalStatus { get; set; }
        public HouseholdMemberType? householdMemberType { get; set; }
        public bool? isAuthorized { get; set; }
        public PersonStatus? status { get; set; }
        public Occupation? occupation { get; set; }
        public string? employer { get; set; }
        public School? school { get; set; }
        public Denomination? denomination { get; set; }
        public string? formerChurch { get; set; }
        public int? barCode { get; set; }
        public int? memberEnvelopeCode { get; set; }
        public string? defaultTagComment { get; set; }
        public bool? thank { get; set; }
        public DateTime? firstRecord { get; set; }
        public PersonAttributes? attributes { get; set; }
        public PersonAddresses? addresses { get; set; }
        public PersonCommunications? communications { get; set; }
        public PersonRequirements? peopleRequirements { get; set; }
        public DateTime? lastMatchDate { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
    }
}
