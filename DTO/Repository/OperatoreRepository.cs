using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity;
using Models.Mappers;
using Models.Repository;
using Models.Tables;
using SysNet;
using System.Diagnostics;
using System.Linq.Expressions;
using Windows.UI;

namespace DTO.Repository
{
    public interface IOperatoreRepository : IBaseRepository<Operatore>
    {
        //Task<bool> EsisteNome(OperatoreDTO dT, CancellationToken ctk = default);
        Task<OperatoreDTO> FirstOperatore(int id, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> LoadByModel(object model, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> LoadOperatori(Expression<Func<Operatore, bool>> predicate, CancellationToken ctk = default);
        Task<bool> Upd(OperatoreDTO dto);
    }

    public class OperatoreRepository : BaseRepository<OperatoreDbContext, Operatore>, IOperatoreRepository
    {
        public async Task<List<OperatoreDTO>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadOperatori(x => x.Id == id);
            else
                return await LoadOperatori(p => p.Id > -2);
        }

        public async Task<List<OperatoreDTO>> LoadOperatori(Expression<Func<Operatore, bool>> predicate
                                                            , CancellationToken ctk = default)
        {
            using OperatoreDbContext _ctx = new();

            // Carichiamo prima gli operatori con i loro dati (Eager Loading)
            return await _ctx.Operatori
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(o => o.Nome)
                .SelectMany(o => o.Permessi.DefaultIfEmpty(), OperatoreDTO.ToOperatoriDtoRelationed) // <--- Usi l'espressione qui
                .ToListAsync(ctk);

        }

        public async Task<bool> Upd(OperatoreDTO dto)
        {
            return await Upd<OperatoreDTO, Operatore>(dto);
        }



        public async Task<OperatoreDTO> FirstOperatore(int id, CancellationToken ctk = default)
        {

            var result = await GetById(id,
                selector: OperatoreDTO.ToOperatoreDto);

            return result ?? new OperatoreDTO();
        }

        

        
        

        public async Task<List<OperatoreDTO>> LoadByModel(object model, CancellationToken ctk = default)
        {

            await Task.FromResult(new List<OperatoreDTO>());
            throw new NotImplementedException();
        }

        //public async Task<bool> EsisteNome(OperatoreDTO dT, CancellationToken ctk = default)
        //{
        //    using OperatoreDbContext _ctx = new();
        //    return await _ctx.Operatori.AnyAsync(p => p.Nome == dT.Nome, ctk);
        //}
    }
}
