using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System.Linq.Expressions;

namespace DTO.Repository
{
    public interface IPostazioneRepository : IBaseRepository<Postazione>
    {
        Task<PostazioneDTO> FirstPostazione(int id);
        Task<List<PostazioneDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<PostazioneDTO>> LoadPostazioni(Expression<Func<Postazione, bool>> predicate, CancellationToken ctk = default);
        Task<List<TipoPostazioneDTO>> LoadTipiPostazione(CancellationToken ctk = default);
        Task<List<TipoRientroDTO>> LoadTipiRientro(CancellationToken ctk = default);
    }

    public class PostazioneRepository : BaseRepository<PostazioneDbContext, Postazione>, IPostazioneRepository
    {
        public async Task<List<PostazioneDTO>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadPostazioni(x => x.Id == id, ctk);
            else
                return await LoadPostazioni(p => p.Id > -2, ctk);
        }

        public async Task<List<PostazioneDTO>> LoadPostazioni(Expression<Func<Postazione, bool>> predicate
                                                            , CancellationToken ctk = default)
        {
            using PostazioneDbContext _ctx = new();

            // Carichiamo prima gli operatori con i loro dati (Eager Loading)
            return await _ctx.Postazioni
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(o => o.Nome)
                .SelectMany(o => o.Reparti.DefaultIfEmpty(), PostazioneDTO.ToPostazioniDtoRelationed) // <--- Usi l'espressione qui
                .ToListAsync(ctk);

        }

        public async Task<PostazioneDTO> FirstPostazione(int id)
        {

            return await GetById(id, selector: PostazioneDTO.ToPostazioneDto) ?? new PostazioneDTO();

        }

        public async Task<List<TipoPostazioneDTO>> LoadTipiPostazione(CancellationToken ctk = default)
        {
            using PostazioneDbContext _ctx = new();
            return await _ctx.TipiPostazione
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoPostazioneDTO.ToDto).ToListAsync(ctk);
        }

        public async Task<List<TipoRientroDTO>> LoadTipiRientro(CancellationToken ctk = default)
        {
            using PostazioneDbContext _ctx = new();
            return await _ctx.TipiRientro
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoRientroDTO.ToDto).ToListAsync(ctk);
        }

    }
}
