using Microsoft.EntityFrameworkCore;
using Models.Context;
using System.Diagnostics;

namespace Models.Repository
{
    public class SettingR : IDisposable
    {
        private readonly SettingDbContext _ctx;

        public SettingR() : base()
        {
            Debug.WriteLine($"***** [Repository] {this.GetType().Name} {this.GetHashCode()} caricato *****");

            _ctx = new SettingDbContext();
        }

#if DEBUG

        ~SettingR()
        {
                // Questo apparirà nella finestra "Output" di Visual Studio
                Debug.WriteLine($"***** [GC] {this.GetType().Name} DISTRUTTO");
        }

#endif



        public virtual void Dispose() { _ctx.Dispose(); }

        public async Task<bool> CheckAppVersion(int appVersion)
        {
            return await _ctx.Settings
                        .AsNoTracking()
                        .AnyAsync(s => s.Version == appVersion);
        }
    }
}
