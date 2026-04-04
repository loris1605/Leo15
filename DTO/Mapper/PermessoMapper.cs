using DTO.Entity;
using Models.Entity;
using Models.Entity.Global;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Mapper
{
    public static  class PermessoMapper
    {
        public static Expression<Func<Permesso, PostazioneXC>> ToPostazioneXC => p => new PostazioneXC
        {
            // Usiamo l'ID direttamente dal Permesso (più sicuro della navigazione)
            CODICEPOSTAZIONE = p.PostazioneId,
            // Usiamo il null-conditional per la navigazione
            DESCPOSTAZIONE = p.Postazione != null ? p.Postazione.Nome : "Nessuna",
            TIPOPOSTAZIONE = p.Postazione != null ? p.Postazione.TipoPostazioneId : 0
        };

        public static Expression<Func<Permesso, PostazioneDTO>> ToPostazioneDTO => p => new PostazioneDTO
        {
            Id = p.Id,
            // Attenzione: qui mappi PostazioneId su CodiceTipoPostazione, 
            // verifica che non debba essere p.Postazione.TipoPostazioneId
            CodiceTipoPostazione = p.PostazioneId,
            NomePostazione = p.Postazione != null ? p.Postazione.Nome : "N/A"
        };
    }
}
