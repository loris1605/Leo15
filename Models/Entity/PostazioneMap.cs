using Models.Interfaces;
using Models.Tables;

namespace Models.Entity
{
    public class PostazioneMap : BaseMap, IMap, IMappable<Postazione>
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

        public Postazione ToTable() => Mappers.PostazioneMapper.ToTable(this);

        public void UpdateTable(Postazione existing)
        {
            if (existing == null) return;

            // Mappatura campi semplici
            existing.Nome = this.NomePostazione;

            // Mappatura Foreign Key (Assicurati che questi ID siano validi nel DB)
            existing.TipoPostazioneId = this.CodiceTipoPostazione;

            // Se TipoRientroId nella tabella Postazione è Nullable (int?), 
            // e il codice è 0, valuta se assegnare null
            existing.TipoRientroId = this.CodiceTipoRientro > 0 ? this.CodiceTipoRientro : -1;

            // NOTA: CodiceReparto non viene aggiornato qui. 
            // Se la Postazione deve cambiare Reparto, devi aggiornare la FK specifica 
            // nella tabella Postazione (es. existing.RepartoId = this.CodiceReparto).
        }
    }


}
