using Models.Entity;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Mappers
{
    public static class TipoRientroMapper
    {
        public static Expression<Func<TipoRientro, TipoRientroMap>> ToMap => p => new TipoRientroMap
        {
            Id = p.Id,
            Nome = p.Nome ?? string.Empty,
            Hours = p.DurataOre
        };
    }

}
