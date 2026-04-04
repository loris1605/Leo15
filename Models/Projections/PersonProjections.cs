using Models.Entity;
using Models.Tables;
using SysNet;
using System.Linq.Expressions;

namespace Models.Projections
{
    public static class PersonProjections
    {
        public static PersonMap PeopleToPersonMap(Person p, Socio? s, Tessera? t) => new()
        {
            Id = p.Id,
            Cognome = p.SurName ?? string.Empty,
            Nome = p.FirstName ?? string.Empty,
            Natoil = p.Natoil,

            CodiceSocio = s != null ? s.Id : 0,
            NumeroSocio = s?.NumeroSocio ?? string.Empty,

            CodiceTessera = t != null ? t.Id : 0,
            NumeroTessera = t?.NumeroTessera ?? string.Empty,
            Scadenza = t?.Scadenza ?? 0
        };

        public static Expression<Func<Person, PersonMap>> ToSimplePersonMap => p => new PersonMap
        {
            Id = p.Id,
            Nome = p.FirstName ?? string.Empty,
            Cognome = p.SurName ?? string.Empty,
            Natoil = p.Natoil,
            CodiceUnivoco = p.UniqueParam ?? string.Empty,

            // Inizializziamo a default i campi delle tabelle correlate
            CodiceSocio = 0,
            NumeroSocio = string.Empty,
            CodiceTessera = 0,
            NumeroTessera = string.Empty,
            Scadenza = 0
        };


    }

}

