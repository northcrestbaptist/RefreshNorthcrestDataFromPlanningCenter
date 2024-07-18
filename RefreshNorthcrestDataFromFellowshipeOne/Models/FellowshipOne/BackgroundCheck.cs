namespace RefreshNorthcrestDataFromFellowshipOne.Models.FellowshipOne
{
    public class BackgroundCheck
    {
        public int? trackingNumber { get; set; }
        public DateTime? requestDate { get; set; }
        public BackgroundCheckStatus? backgroundCheckStatus { get; set; }
    }
}
