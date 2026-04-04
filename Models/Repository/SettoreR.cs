using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity;
using Models.Interfaces;
using Models.Mappers;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Repository
{
    public class SettoreR : BaseRepository<SettoreDbContext, Settore>, IGroupQ<SettoreMap>
    {
        public SettoreR() : base() { }

        public async Task<List<SettoreMap>> Load(int id)
        {
            if (id > 0)
                return await LoadSettori(x => x.Id == id);
            else
                return await LoadSettori(p => p.Id > 0);

        }

        public Task<List<SettoreMap>> LoadByModel(object model) =>
                Task.FromResult((List<SettoreMap>)model);

        public async Task<List<SettoreMap>> LoadSettori(Expression<Func<Settore, bool>> predicate)
        {
            using AppDbContext _ctx = new();
            try
            {
                var x =  await _ctx.Settori
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(p => p.Nome)
                .SelectMany(
                    o => o.Listini.DefaultIfEmpty(),
                    SettoreMapper.ToSettoreMap// Usiamo la proiezione statica
                )
                .ToListAsync();
                return x;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in OnLoading: {ex.Message}");
                return [];
            }
            
            
            
        }

        public async Task<List<TipoSettoreMap>> LoadTipiSettore()
        {
            using SettoreDbContext _ctx = new();
            return await _ctx.TipiSettore
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoSettoreMapper.ToMap).ToListAsync();
        }

        public async Task<SettoreMap> GetById(int id) =>
                        await base.GetById(id, SettoreMapper.ToSimpleSettoreMap);
    }
}
