using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity;
using Models.Interfaces;
using Models.Mappers;
using Models.Tables;
using System.Linq.Expressions;

namespace Models.Repository
{
    public class PostazioneR : BaseRepository<AppDbContext, Postazione>, IGroupQ<PostazioneMap>
    {
        public PostazioneR() : base() { }
       

        public async Task<List<PostazioneMap>> Load(int id)
        {
            if (id > 0)
                return await LoadPostazioni(x => x.Id == id);
            else
                return await LoadPostazioni(p => p.Id > -1);

        }

        public Task<List<PostazioneMap>> LoadByModel(object model) =>
                Task.FromResult((List<PostazioneMap>)model);

        public async Task<List<PostazioneMap>> LoadPostazioni(Expression<Func<Postazione, bool>> predicate)
        {
            using AppDbContext _ctx = new();
            return await _ctx.Postazioni
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(p => p.Nome)
                .SelectMany(
                    o => o.Reparti.DefaultIfEmpty(),
                    PostazioneMapper.ToPostazioneMap// Usiamo la proiezione statica
                )
                .ToListAsync();
        }

        public async Task<List<TipoPostazioneMap>> LoadTipiPostazione()
        {
            using AppDbContext _ctx = new();
            return await _ctx.TipiPostazione
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoPostazioneMapper.ToMap).ToListAsync();
        }

        public async Task<List<TipoRientroMap>> LoadTipiRientro()
        {
            using AppDbContext _ctx = new();
            return await _ctx.TipiRientro
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(TipoRientroMapper.ToMap).ToListAsync();
        }

        public async Task<PostazioneMap> GetById(int id) =>
                        await base.GetById(id, PostazioneMapper.ToSimplePostazioneMap);

        

    }
}
