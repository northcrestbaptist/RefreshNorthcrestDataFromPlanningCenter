using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class PersonAddress
    {
        [JsonProperty("@id")]
        public int? id { get; set; }
        public HouseholdId? household { get; set; }
        public AddressType? addressType { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? address3 { get; set; }
        public string? city { get; set; }
        public string? postalCode { get; set; }
        public string? county { get; set; }
        public string? country { get; set; }
        public string? stProvince { get; set; }
        public string? carrierRoute { get; set; }
        public string? deliveryPoint { get; set; }
        public DateTime? addressDate { get; set; }
        public string? addressComment { get; set; }
        public bool? uspsVerified { get; set; }
        public DateTime? addressVerifiedDate { get; set; }
        public DateTime? lastVerificationAttemptDate { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
    }
}
