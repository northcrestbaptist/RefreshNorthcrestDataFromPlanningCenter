using System;

namespace Northcrest.Domain.FellowshipOne
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Type { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public DateTime AddressDate { get; set; }
        public Person Person { get; set; }
        public int PersonId { get; set; }
    }
}
