using System;

namespace Northcrest.Domain.FellowshipOne
{
    public class BackgroundCheck
    {
        public int BackgroundCheckId { get; set; }
        public int TrackingNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status {  get; set; }

        public Person Person { get; set; }
        public int PersonId { get; set; }
    }
}
