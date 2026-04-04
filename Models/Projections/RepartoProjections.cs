using Models.Entity.Global;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Projections
{
    public static class RepartoProjections
    {
        public static Expression<Func<Reparto, SettoreXC>> ToSettoreXC => p => new SettoreXC
        {
            CODICESETTORE = p.SettoreId,
            DESCSETTORE = p.Settore!.Label!,
            NOMESETTORE = p.Settore.Nome!
        };
    }
}
