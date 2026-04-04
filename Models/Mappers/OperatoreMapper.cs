using Models.Entity;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Mappers
{
    public static class OperatoreMapper
    {
        public static OperatoreMap ToMap(this Operatore entity)
        {
            if (entity == null) return new OperatoreMap();

            return new OperatoreMap
            {
                Id = entity.Id,
                NomeOperatore = entity.Nome ?? string.Empty,
                Password = entity.Password ?? string.Empty,
                Abilitato = entity.Abilitato,
                Badge = entity.Pass,
                CodicePerson = entity.PersonId
            };
        }

        public static Operatore ToTable(this OperatoreMap map)
        {
            if (map == null) return new Operatore();

            return new Operatore
            {
                Id = map.Id,
                Nome = map.NomeOperatore,
                Password = map.Password,
                Abilitato = map.Abilitato,
                Pass = map.Badge,
                PersonId = map.CodicePerson
            };
        }


        

        public static Expression<Func<Operatore, OperatoreMap>> ToSimpleOperatoreMap => o => new OperatoreMap
        {
            Id = o.Id,
            NomeOperatore = o.Nome,
            Password = o.Password,
            Abilitato = o.Abilitato,
            Badge = o.Pass


        };

        public static Expression<Func<Operatore, Permesso?, OperatoreMap>> ToOperatoreMap =>
        (o, p) => new OperatoreMap
        {
            Id = o.Id,
            NomeOperatore = o.Nome,
            Password = o.Password,
            Abilitato = o.Abilitato,
            Badge = o.Pass,
            CodicePerson = o.PersonId,
            CodicePermesso = p != null ? p.Id : 0,
            // Uso dell'operatore condizionale per evitare crash se i rami della relazione sono null
            NomePostazione = p != null && p.Postazione != null ? p.Postazione.Nome : "Nessuna",
            TipoPostazione = p != null && p.Postazione != null && p.Postazione.TipoPostazione != null
                             ? p.Postazione.TipoPostazione.Nome
                             : "N/A"
        };
    }
}
