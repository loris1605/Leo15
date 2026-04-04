using Models.Entity;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Mappers
{
    public static class PermessoMapper
    {
        

        public static Expression<Func<Permesso, PostazioneMap>> ToPostazioneMap => p => new PostazioneMap
        {
            Id = p.Id,
            // Attenzione: qui mappi PostazioneId su CodiceTipoPostazione, 
            // verifica che non debba essere p.Postazione.TipoPostazioneId
            CodiceTipoPostazione = p.PostazioneId,
            NomePostazione = p.Postazione != null ? p.Postazione.Nome : "N/A"
        };

        //public static Expression<Func<Postazione, PostazioneElencoMap>> ToPostazioneElencoMap => o =>
        //   new PostazioneElencoMap
        //   {
        //       Id = o.Id,
        //       NomePostazione = o.Nome,
        //       CodiceTipoPostazione = o.TipoPostazioneId,
        //       NomeTipoPostazione = o.TipoPostazione != null ? o.TipoPostazione.Nome : "N/A",
        //       //HasPermesso = o.Permessi.Any()

        //   };

        public static PostazioneElencoMap ToMap(this Postazione o)
        {
            return new PostazioneElencoMap
            {
                Id = o.Id,
                NomePostazione = o.Nome,
                CodiceTipoPostazione = o.TipoPostazioneId,
                NomeTipoPostazione = o.TipoPostazione != null ? o.TipoPostazione.Nome : "N/A" ,
                HasPermesso = o.Permessi.Any(p => p.OperatoreId == o.Id)
            };
        }   


    }
}
