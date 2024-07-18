
namespace Northcrest.Domain.FellowshipOne
{
    public class Communication
    {
        public int CommunicationId { get; set; }
        public string SpecificType { get; set; }
        public string GeneralType { get; set; }
        public string Value { get; set; }
        public string SearchValue { get; set; }
        public bool Preferred { get; set; }
        public string Comment { get; set; }
        public Person Person { get; set; }
        public int PersonId { get; set; }
    }
}
