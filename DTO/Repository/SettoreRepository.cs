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
    public interface ISettoreRepository : IBaseRepository<Settore>
    {
        Task<SettoreDTO> FirstSettore(int id, CancellationToken ctk = default);
        Task<List<SettoreDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<SettoreDTO>> LoadSettori(Expression<Func<Settore, bool>> predicate, CancellationToken ctk = default);
        Task<bool> Upd(SettoreDTO dto, CancellationToken ctk = default);
        Task<List<TipoSettoreDTO>> LoadTipiSettore(CancellationToken ctk = default);
        Task<List<TariffaDTO>> GetListini(int id, CancellationToken ctk = default);
    }

    public class SettoreRepository : BaseRepository<SettoreDbContext, Settore>, ISettoreRepository
    {
        public async Task<List<SettoreDTO>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadSettori(x => x.Id == id, ctk);
            else
                return await LoadSettori(p => p.Id > 0, ctk);
        }

        public async Task<List<SettoreDTO>> LoadSettori(Expression<Func<Settore, bool>> predicate
                                                            , CancellationToken ctk = default)
        {
            using SettoreDbContext _ctx = new();

            // Carichiamo prima gli operatori con i loro dati (Eager Loading)
            return await _ctx.Settori
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(o => o.Nome)
                .SelectMany(o => o.Listini.DefaultIfEmpty(), SettoreDTO.ToSettoriDtoRelationed) // <--- Usi l'espressione qui
                .ToListAsync(ctk);

        }

        public async Task<List<TipoSettoreDTO>> LoadTipiSettore(CancellationToken ctk = default)
        {
            using SettoreDbContext _ctx = new();
            return await _ctx.TipiSettore
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoSettoreDTO.ToDto).ToListAsync(ctk);


        }

        public async Task<SettoreDTO> FirstSettore(int id, CancellationToken ctk = default) 
        {
            return await GetById(id, selector: SettoreDTO.ToSettoreDto, ctk: ctk) ?? new SettoreDTO();    
        }

        public async Task<bool> Upd(SettoreDTO dto, CancellationToken ctk = default) => await Upd<SettoreDTO, Settore>(dto, ctk);

        public async Task<List<TariffaDTO>> GetListini(int id, CancellationToken ctk = default)
        {
            using SettoreDbContext _ctx = new();
            return await _ctx.Tariffe
                .AsNoTracking()
                .Select(TariffaDTO.ToTariffaElencoDto(id)) // Passi l'id qui
                .ToListAsync(ctk);
        }
    }
}
