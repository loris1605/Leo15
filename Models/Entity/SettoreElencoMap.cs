namespace Models.Entity
{
    public class SettoreElencoMap : BaseMap, IMap
    {
        public string NomeSettore { get; set; } = string.Empty;
        public string EtichettaSettore { get; set; } = string.Empty;
        public int CodiceTipoSettore { get; set; } = 0;
        public string NomeTipoSettore { get; set; } = string.Empty;
        public bool HasPermesso { get; set; } = false;

        public override string Nome => NomeSettore;

    }
}
