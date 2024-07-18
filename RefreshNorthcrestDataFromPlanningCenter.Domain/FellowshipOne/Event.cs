using System;

namespace Northcrest.Domain.FellowshipOne
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public Person Person { get; set; }
        public int PersonId { get; set; }
    }
}
