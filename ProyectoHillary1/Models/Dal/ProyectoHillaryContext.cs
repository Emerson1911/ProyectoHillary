using Microsoft.EntityFrameworkCore;
using ProyectoHillary1.Models.En;

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
        public virtual DbSet<Tarea> tarea { get; set; }
    }
}
