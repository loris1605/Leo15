using Models.Entity.Global;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Mapper
{
    public static class RepartoMapper
    {
        public static Expression<Func<Reparto, SettoreXC>> ToSettoreXC => r => new SettoreXC
        {
            // Usiamo l'ID direttamente dal Reparto (FK) per sicurezza
            CODICESETTORE = r.SettoreId,

            // Protezione contro i null per le proprietà di navigazione
            DESCSETTORE = r.Settore != null ? r.Settore.Label : "DESCRIZIONE MANCANTE",
            NOMESETTORE = r.Settore != null ? r.Settore.Nome : "SETTORE NON ASSEGNATO"
        };
    }
}
