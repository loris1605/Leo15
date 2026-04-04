namespace Models.Entity
{
    public class SchedaMap : BaseMap, IMap
    {
        public int Posizione { get; set; }
        public int CodiceTessera { get; set; }
        public int NumeroTessera { get; set; }
        public int CodiceSocio { get; set; }
        public string NumeroSocio { get; set; } = string.Empty;
        public int CodicePerson { get; set; }
        public string Cognome { get; set; } = string.Empty;
        public int Natoil { get; set; }
        public DateTime CheckInTime { get; set; } = DateTime.Now;
        public bool Blocco { get; set; } = false;
        public string Note { get; set; } = string.Empty;
        public decimal Consumazione { get; set; } = decimal.Zero;
        
    }
}
