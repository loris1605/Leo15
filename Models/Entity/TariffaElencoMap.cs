namespace Models.Entity
{
    public class TariffaElencoMap : BaseMap, IMap
    {
        public string NomeTariffa { get; set; } = string.Empty;
        public string EtichettaTariffa { get; set; } = string.Empty;
        public decimal PrezzoTariffa { get; set; } = decimal.Zero;
        public bool IsFreeDrink { get; set; }
        public bool HasListino { get; set; }

        public override string Nome => NomeTariffa;

    }
}
