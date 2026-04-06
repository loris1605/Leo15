using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity;
using Models.Interfaces;
using Models.Mappers;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Repository
{
    public class TariffaR : BaseRepository<TariffaDbContext, Tariffa>, IGroupQ<TariffaMap>
    {
        public TariffaR() : base() { }

        public async Task<List<TariffaMap>> Load(int id, CancellationToken ctk = default)
        {
            if (id > 0)
                return await LoadTariffe(x => x.Id == id);
            else
                return await LoadTariffe(p => p.Id > 0);

        }

        public Task<List<TariffaMap>> LoadByModel(object model, CancellationToken ctk = default) =>
                Task.FromResult((List<TariffaMap>)model);

        public async Task<List<TariffaMap>> LoadTariffe(Expression<Func<Tariffa, bool>> predicate)
        {
            using TariffaDbContext _ctx = new();
            try
            {
                var x = await _ctx.Tariffe
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(p => p.Nome)
                .Select(TariffaMapper.ToSimpleTariffaMap)// Usiamo la proiezione statica
                .ToListAsync();
                return x;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in OnLoading: {ex.Message}");
                return [];
            }

        }

        public async Task<TariffaMap> GetById(int id) =>
                        await base.GetById(id, TariffaMapper.ToSimpleTariffaMap);
    }
}
