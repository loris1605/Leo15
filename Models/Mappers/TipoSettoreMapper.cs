using Models.Entity;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Models.Mappers
{
    public static class TipoSettoreMapper
    {
        public static Expression<Func<TipoSettore, TipoSettoreMap>> ToMap => t => new TipoSettoreMap
        {
            Id = t.Id,
            Nome = t.Nome ?? string.Empty
        };
    }

}
