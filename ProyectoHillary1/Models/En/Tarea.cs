using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoHillary1.Models.En
{
    [Table("tareas")]
    public partial class Tarea
    {
        public int Id { get; set; }
        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public virtual Empresa Empresa { get; set; } = null!;
    }
}
