using Models.Entity;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Mappers
{
    public static class TariffaMapper
    {
        public static TariffaMap ToMap(this Tariffa entity)
        {
            if (entity == null) return new TariffaMap();
            return new TariffaMap
            {
                Id = entity.Id,
                NomeTariffa = entity.Nome ?? string.Empty,
                EtichettaTariffa = entity.Label ?? string.Empty,
                PrezzoTariffa = entity.Prezzo
            };
        }

        public static Tariffa ToTable(this TariffaMap map)
        {
            if (map == null) return new Tariffa();
            return new Tariffa
            {
                Id = map.Id,
                Nome = map.NomeTariffa,
                Label = map.EtichettaTariffa,
                Prezzo = map.PrezzoTariffa
            };
        }

        public static Expression<Func<Tariffa, TariffaMap>> ToSimpleTariffaMap => s =>
           new TariffaMap
           {
               Id = s.Id,
               NomeTariffa = s.Nome,
               EtichettaTariffa = s.Label,
               PrezzoTariffa = s.Prezzo,
               HasListino = s.Listini.Any()
           };
    }
}
