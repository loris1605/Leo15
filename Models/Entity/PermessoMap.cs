using Models.Interfaces;

namespace Models.Entity
{
    public class PermessoMap : BaseMap, IMap
    {
        public int CodiceOperatore { get; set; }
        public int CodicePostazione { get; set; }
        public int CodiceTipoPostazione { get; set; }
        public string NomePostazione { get; set; } = string.Empty;
        public int CodiceTipoRientro { get; set; }
        public int OreDurataRientro { get; set; }

        // Colleghiamo Nome a NomePostazione (Getter e Setter)
        public override string Nome
        {
            get => NomePostazione;
            set => NomePostazione = value ?? string.Empty;
        }

        public override string? Titolo => $"{NomePostazione} (Rientro: {OreDurataRientro}h)";

    }
}
