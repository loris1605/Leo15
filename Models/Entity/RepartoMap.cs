namespace Models.Entity
{
    public class RepartoMap : BaseMap, IMap
    {
        // FK verso Postazione e Settore
        public int CodicePostazione { get; set; }
        public int CodiceSettore { get; set; }

        // Dati descrittivi che trasciniamo dal Settore per la UI
        public string NomeSettore { get; set; } = string.Empty;
        public string EtichettaSettore { get; set; } = string.Empty;

        // Poiché il reparto non ha un nome proprio, usiamo quello del settore 
        // come rappresentante per l'interfaccia IMap
        public override string Nome
        {
            get => NomeSettore;
            set => NomeSettore = value ?? string.Empty;
        }

        // Il Titolo mostrerà l'identificativo del legame
        public override string? Titolo => $"Legame Settore: {NomeSettore} (ID Postazione: {CodicePostazione})";
    }

}
