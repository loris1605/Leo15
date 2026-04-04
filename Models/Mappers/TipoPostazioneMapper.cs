using Models.Entity;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Mappers
{
    public static class TipoPostazioneMapper
    {
        public static Expression<Func<TipoPostazione, TipoPostazioneMap>> ToMap => t => new TipoPostazioneMap
        {
            Id = t.Id,
            Nome = t.Nome
        };
    }
}
