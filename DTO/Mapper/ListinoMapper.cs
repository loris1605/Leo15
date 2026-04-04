using Models.Entity.Global;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Mapper
{
    public static class ListinoMapper
    {
        public static Expression<Func<Listino, TariffaXC>> ToTariffaXC => p => new TariffaXC
        {
            CODICETARIFFA = p.TariffaId,
            DESCTARIFFA = p.Tariffa!.Label,
            PRICETARIFFA = p.Tariffa.Prezzo
        };
    }
}
