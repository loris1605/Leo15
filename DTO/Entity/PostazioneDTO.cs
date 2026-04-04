namespace DTO.Entity
{
    public class PostazioneDTO : BaseDTO, IDTO
    {
        public int CodiceTipoPostazione { get; set; }
        public string NomePostazione { get; set; } = string.Empty;
        public string NomeTipoPostazione { get; set; } = string.Empty;
        public int CodiceReparto { get; set; }
        public string NomeSettore { get; set; } = string.Empty;
        public string EtichettaSettore { get; set; } = string.Empty;
        public string NomeTipoSettore { get; set; } = string.Empty;
        public int CodiceTipoRientro { get; set; }
        public string NomeTipoRientro { get; set; } = string.Empty;
        public bool HasPermesso { get; set; }

        public override string Nome
        {
            get => NomePostazione;
            set => NomePostazione = value ?? string.Empty;
        }

        public override string? Titolo => $"{NomePostazione} - {NomeTipoPostazione}";
    }
}
