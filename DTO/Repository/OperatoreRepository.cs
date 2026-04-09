using DTO.Entity;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Entity;
using Models.Mappers;
using Models.Repository;
using Models.Tables;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DTO.Repository
{
    public interface IOperatoreRepository
    {
        Task<int> Add(OperatoreDTO map, CancellationToken ctk = default);
        Task<bool> Del(OperatoreDTO map, CancellationToken ctk = default);
        Task<OperatoreDTO> FirstOperatore(int id, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> Load(int id, CancellationToken ctk = default);
        Task<List<OperatoreDTO>> LoadOperatori(Expression<Func<Operatore, bool>> predicate);
        Task<bool> Upd(OperatoreDTO map, CancellationToken ctk = default);
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

        public async Task<List<OperatoreDTO>> LoadOperatori(Expression<Func<Operatore, bool>> predicate)
        {
            using OperatoreDbContext _ctx = new();
            return await _ctx.Operatori
                .AsNoTracking()
                .Where(predicate)
                .OrderBy(p => p.Nome)
                .SelectMany(
                    o => o.Permessi.DefaultIfEmpty(),
                    (o, p) => new OperatoreDTO // Scritta direttamente qui
                    {
                        Id = o.Id,
                        NomeOperatore = o.Nome,
                        Password = o.Password,
                        Abilitato = o.Abilitato,
                        Badge = o.Pass,
                        CodicePerson = o.PersonId,
                        CodicePermesso = p != null ? p.Id : 0,
                        NomePostazione = p != null && p.Postazione != null ? p.Postazione.Nome : "Nessuna",
                        TipoPostazione = p != null && p.Postazione != null && p.Postazione.TipoPostazione != null
                                         ? p.Postazione.TipoPostazione.Nome
                                         : "N/A"
                    }
                )
                .ToListAsync();
        }

        public async Task<OperatoreDTO> FirstOperatore(int id, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            using OperatoreDbContext _ctx = new();

            var result = await _ctx.Operatori
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new OperatoreDTO // Proiezione esplicita (Traducibile in SQL)
                {
                    Id = p.Id,
                    NomeOperatore = p.Nome,
                    Password = p.Password,
                    Abilitato = p.Abilitato,
                    Badge = p.Pass

                    // Aggiungi qui solo i campi necessari per il "Simple" DTO
                })
                .FirstOrDefaultAsync(ctk);

            ctk.ThrowIfCancellationRequested();

            return result ?? new OperatoreDTO();
        }

        public async Task<int> Add(OperatoreDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            using OperatoreDbContext _ctx = new();

            var operatore = new Operatore
            {
                Nome = map.Nome,
                Password = map.Password,
                Abilitato = map.Abilitato,
                Pass = map.Badge
            };

            await _ctx.Operatori.AddAsync(operatore, ctk);

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return operatore.Id;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add Operatore: {ex.InnerException?.Message ?? ex.Message}");
                return -1;
            }
        }

        public async Task<bool> Upd(OperatoreDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            using OperatoreDbContext _ctx = new();

            var rec = await _ctx.Operatori.FindAsync(map.Id, ctk);
            if (rec == null) return false;

            // 2. Aggiorniamo le proprietà
            rec.Nome = map.NomeOperatore;
            rec.Password = map.Password;
            rec.Pass = map.Badge;
            rec.Abilitato = map.Abilitato;

            ctk.ThrowIfCancellationRequested();

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Modifica Operatore annullato dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }

        }
        public async Task<bool> Del(OperatoreDTO map, CancellationToken ctk = default)
        {
            ctk.ThrowIfCancellationRequested();
            using OperatoreDbContext _ctx = new();

            var rec = await _ctx.Operatori.FindAsync(map.Id, ctk);
            if (rec == null) return false;

            _ctx.Operatori.Remove(rec);

            ctk.ThrowIfCancellationRequested();

            try
            {
                await _ctx.SaveChangesAsync(ctk);
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine(">>> [INFO] Cancellazione Operatore annullato dall'utente.");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Add: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
        }
    }
}
