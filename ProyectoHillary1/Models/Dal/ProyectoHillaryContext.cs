using ProyectoHillary1.Models.En;
using Microsoft.EntityFrameworkCore;

namespace ProyectoHillary1.Models.Dal
{
    public class ProyectoHillaryContext : DbContext
    {
        public ProyectoHillaryContext(DbContextOptions<ProyectoHillaryContext> options) : base(options)
        {
        }

        public virtual DbSet<Rol> rol { get; set; }
        public virtual DbSet<Empresa> empresa { get; set; }
        public virtual DbSet<Usuario> usuario { get; set; }
    }
}
