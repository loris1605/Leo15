using Microsoft.EntityFrameworkCore;
using Models.Tables;

namespace Models.Context
{
    public class SettingDbContext : BaseContext
    {
        public DbSet<DbSettings> Settings { get; set; } = null!;
    }
}
