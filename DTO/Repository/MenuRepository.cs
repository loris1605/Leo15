using DTO.Entity;
using DTO.Mapper;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;

namespace DTO.Repository
{
    public interface IMenuRepository : IDisposable
    {
        Task<List<PostazioneDTO>> CaricaPostazioniCassa(int CodiceOperatore);
        Task<bool> EsisteGiornataAperta();
    }

    public class MenuRepository : BaseRepository<MenuDbContext, Permesso>, IMenuRepository
    {
        public async Task<bool> EsisteGiornataAperta()
        {
            using MenuDbContext _ctx = new();
            return await _ctx.Giornate.AnyAsync(p => p.Aperta);
        }

        public async Task<List<PostazioneDTO>> CaricaPostazioniCassa(int CodiceOperatore)
        {
            using MenuDbContext _ctx = new();
            IQueryable<Permesso> query =
                _ctx.Permessi
                    .AsNoTracking()
                    .Where(p => p.OperatoreId == CodiceOperatore)
                    .Where(p => p.Postazione!.TipoPostazioneId == 2)
                    .Where(p => p.PostazioneId > 0);

            return await query.Select(PermessoMapper.ToPostazioneDTO).ToListAsync();

        }
    }
}
