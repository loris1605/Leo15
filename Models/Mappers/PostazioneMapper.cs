using Models.Entity;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Mappers
{
    public static class PostazioneMapper
    {
        public static PostazioneMap ToMap(this Postazione entity)
        {
            return new PostazioneMap
            {
                Id = entity.Id,
                NomePostazione = entity.Nome, // Mappa Nome -> NomePostazione
                CodiceTipoPostazione = entity.TipoPostazioneId,
                CodiceTipoRientro = entity.TipoRientroId
            };
        }

        public static Postazione ToTable(this PostazioneMap map)
        {
            return new Postazione
            {
                Id = map.Id,
                Nome = map.NomePostazione,
                TipoPostazioneId = map.CodiceTipoPostazione,
                TipoRientroId = map.CodiceTipoRientro
            };
        }

        public static Expression<Func<Postazione, Reparto?, PostazioneMap>> ToPostazioneMap =>
        (o, p) => new PostazioneMap
        {
            Id = o.Id,
            // Protezione su TipoPostazione
            CodiceTipoPostazione = o.TipoPostazione != null ? o.TipoPostazione.Id : 0,
            NomePostazione = o.Nome,
            NomeTipoPostazione = o.TipoPostazione != null ? o.TipoPostazione.Nome : "N/A",
        
            // Protezione su Reparto
            CodiceReparto = p != null ? p.Id : 0,
        
            // Protezione a cascata su Settore (p -> p.Settore)
            NomeSettore = (p != null && p.Settore != null) ? p.Settore.Nome : "Nessuno",
            EtichettaSettore = (p != null && p.Settore != null) ? p.Settore.Label : "Nessuna",
        
            // Protezione profonda (p -> p.Settore -> p.Settore.TipoSettore)
            NomeTipoSettore = (p != null && p.Settore != null && p.Settore.TipoSettore != null)
                              ? p.Settore.TipoSettore.Nome
                              : "N/A",
            HasPermesso = o.Permessi.Any()
        };

        public static Expression<Func<Postazione, PostazioneMap>> ToSimplePostazioneMap => o => 
           new PostazioneMap
            {
                Id = o.Id,
                NomePostazione = o.Nome,
                CodiceTipoPostazione = o.TipoPostazioneId,
                CodiceTipoRientro = o.TipoRientroId,
                HasPermesso = o.Permessi.Any()

           };
    }
}
