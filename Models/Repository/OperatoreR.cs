using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity;
using Models.Interfaces;
using Models.Mappers;
using Models.Projections;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Repository
{
    public class OperatoreR : BaseRepository<AppDbContext, Operatore>, IGroupQ<OperatoreMap>
    {
        private static int classCount;

        public OperatoreR() : base()
        {
            System.Diagnostics.Debug.WriteLine($"***** [VM] {this.GetType().Name} " +
                                               $"#{Interlocked.Increment(ref classCount)} " +
                                               $"caricato *****");

        }

        public async Task<List<OperatoreMap>> Load(int id)
        {
            if (id > 0)
                return await LoadOperatori(x => x.Id == id);
            else
                return await LoadOperatori(p => p.Id > -2);
        }

        public Task<List<OperatoreMap>> LoadByModel(object model) =>
                Task.FromResult((List<OperatoreMap>)model);

        public async Task<List<OperatoreMap>> LoadOperatori(Expression<Func<Operatore, bool>> predicate)
        {
            using AppDbContext _ctx = new();
            return await _ctx.Operatori
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(p => p.Nome)
                .SelectMany(
                    o => o.Permessi.DefaultIfEmpty(),
                    OperatoreMapper.ToOperatoreMap // Usiamo la proiezione statica
                )
                .ToListAsync();
        }

        public async Task<OperatoreMap> GetById(int id) =>
                        await base.GetById(id, OperatoreMapper.ToSimpleOperatoreMap);
        

    }
}
