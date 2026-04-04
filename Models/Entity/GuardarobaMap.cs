namespace Models.Entity
{
    public class GuardarobaMap  
    {
        public int IdScheda { get; set; }
        public int Grb1 { get; set; }
        public int Grb2 { get; set; }
        public int Grb3 { get; set; }
        public int Grb4 { get; set; }
        public string Cognome { get; set; } = string.Empty;
        public string Nome {  get; set; } = string.Empty;
        public int Posizione { get; set; }

        
    }
}
