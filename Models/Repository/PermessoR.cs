using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity;
using Models.Mappers;
using Models.Tables;

namespace Models.Repository
{
    public class PermessoR : BaseRepository<PermessoDbContext, Permesso>
    {
        public PermessoR() : base() { }

        public async Task<List<PostazioneElencoMap>> GetPostazioniSenzaPermesso(int operatoreId)
        {
            using PermessoDbContext _ctx = new();
            // Recuperiamo tutte le postazioni
            var tutteLePostazioni = await _ctx.Postazioni.ToListAsync();

            // Filtriamo quelle che non hanno un permesso per l'operatore indicato
            var postazioniSenzaPermesso = tutteLePostazioni
                .Where(p => !_ctx.Permessi
                    .Any(perm => perm.PostazioneId == p.Id && perm.OperatoreId == operatoreId))
                .Select(PermessoMapper.ToMap)
                .ToList();

            return postazioniSenzaPermesso;
        }

    }
}
