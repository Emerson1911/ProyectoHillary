using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoHillary1.Models.En
{
    public partial class Usuario
    {
        public int Id { get; set; }
        [Column("empresa_id")]  // ✅ Aquí está el mapeo
        public int EmpresaId { get; set; }
        [Column("rol_id")]  // ✅ Aquí está el mapeo
        public int RolId { get; set; }

        public string? Nombre { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public bool? Activo { get; set; }

        public virtual Empresa Empresa { get; set; } = null!;

        public virtual Rol Rol { get; set; } = null!;
    }
}
