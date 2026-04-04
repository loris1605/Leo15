using Models.Entity;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Mappers
{
    public static class SettoreMapper
    {
        public static SettoreMap ToMap(this Settore entity)
        {
            if (entity == null) return new SettoreMap();
            return new SettoreMap
            {
                Id = entity.Id,
                NomeSettore = entity.Nome ?? string.Empty,
                EtichettaSettore = entity.Label ?? string.Empty,
                CodiceTipoSettore = entity.TipoSettoreId
            };
        }

        public static Settore ToTable(this SettoreMap map)
        {
            if (map == null) return new Settore();
            return new Settore
            {
                Id = map.Id,
                Nome = map.NomeSettore,
                Label = map.EtichettaSettore,
                TipoSettoreId = map.CodiceTipoSettore
            };
        }

        public static Expression<Func<Settore, Listino?, SettoreMap>> ToSettoreMap =>
        (s, l) => new SettoreMap
        {
            Id = s.Id,
            // Usiamo l'ID diretto se disponibile, altrimenti navigazione protetta
            CodiceTipoSettore = s.TipoSettoreId != 0 ? s.TipoSettoreId : (s.TipoSettore != null ? s.TipoSettore.Id : 0),
            NomeSettore = s.Nome,
            EtichettaSettore = s.Label,
            NomeTipoSettore = s.TipoSettore != null ? s.TipoSettore.Nome : "Nessun Tipo",

            // Protezione su Listino
            CodiceListino = l != null ? l.Id : 0,

            // Protezione a cascata (EF traduce correttamente questi null check in SQL CASE)
            NomeTariffa = (l != null && l.Tariffa != null) ? l.Tariffa.Nome : "Nessuna",
            EtichettaTariffa = (l != null && l.Tariffa != null) ? l.Tariffa.Label : "Nessuna",
            PrezzoTariffa = (l != null && l.Tariffa != null) ? l.Tariffa.Prezzo : 0m,
            HasReparto = s.Reparti.Any()
        };

        public static Expression<Func<Settore, SettoreMap>> ToSimpleSettoreMap => s =>
           new SettoreMap
           {
               Id = s.Id,
               NomeSettore = s.Nome,
               EtichettaSettore = s.Label,
               CodiceTipoSettore = s.TipoSettoreId,
               HasReparto = s.Reparti.Any()
           };
    }

}
