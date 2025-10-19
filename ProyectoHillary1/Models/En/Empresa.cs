using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace ProyectoHillary1.Models.En
{
    public partial class Empresa
    {
        public int Id { get; set; }

        public string? Nombre { get; set; }

        public string? Ruc { get; set; }

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public bool? Activo { get; set; }
        [Column("created_at")]  // ✅ Aquí está el mapeo
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]  // ✅ Aquí está el mapeo
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
