using Microsoft.EntityFrameworkCore;
using Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Context
{
    public class SchedaDbContext : BaseContext
    {
        public DbSet<Scheda> Schede { get; set; } = null!;
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Socio> Soci { get; set; } = null!;
        public DbSet<Tessera> Tessere { get; set; } = null!;
    }
}
