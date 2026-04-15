using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class MenuDbContext : BaseContext
    {
        public DbSet<Giornata> Giornate { get; set; } = null!;
        public DbSet<TipoPostazione> TipiPostazione { get; set; } = null!;
        public DbSet<Postazione> Postazioni { get; set; } = null!;
        public DbSet<Permesso> Permessi { get; set; } = null!;

    }
}
