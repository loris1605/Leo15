using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Repository
{
    public interface IPostazioneRepository : IBaseRepository<Postazione>
    {
        Task<PostazioneDTO> FirstPostazione(int id);
        Task<List<PostazioneDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<PostazioneDTO>> LoadPostazioni(Expression<Func<Postazione, bool>> predicate, CancellationToken ctk = default);
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
    }
}
