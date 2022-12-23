using NorthcrestWebService.Models.Interfaces;

namespace NorthcrestWebService.Models
{
    public class ClientPlan_ForSongs : IClientPlan_ForSongs
    {
        public int Plan_ForSongsId { get; set; }
        public int PlanId { get; set; }
        public string Type { get; set; }
        public DateTime PlanDateTime { get; set; }
        public List<IClientPlanSong> PlanSongs { get; set; } = new List<IClientPlanSong>();
    }
}
