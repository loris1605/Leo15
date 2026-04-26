using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Repository;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Repository
{
    public interface ISchedaRepository
    {
        Task<List<SchedaDTO>> SociDentro(CancellationToken ctk = default);
    }

    public class SchedaRepository : BaseRepository<SchedaDbContext, Scheda>, ISchedaRepository
    {
        public async Task<List<SchedaDTO>> SociDentro(CancellationToken ctk = default)
        {
            using SchedaDbContext _ctx = new();
            var data = await _ctx.Schede
                .AsNoTracking()
                .OrderBy(o => o.Id)
                .Select(SchedaDTO.ToSchedaDto)
                .ToListAsync(ctk);
            return data;
        }

    }
}
