using Models.Interfaces;

namespace Models.Entity
{
    public class SchedaContoMap : BaseMap, IMap
    {
        public int IdScheda { get; set; }
        public string DescSettore { get; set; } = string.Empty;
        public string DescPostazione {  get; set; } = string.Empty;
        public string VoiceDesc { get; set; } = string.Empty;
        public decimal VoicePrice { get; set; } = decimal.Zero;
        public bool Pagato { get; set; } = false;
        public string Note {  get; set; } = string.Empty;
        public DateTime DataOra { get; set; } = DateTime.Now;
        public string Nickname {  get; set; } = string.Empty;
        public int IdOperatore { get; set; }

    }
}
