namespace ProyectoHillary1.Models.En
{
    public partial class Tarea
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public virtual Empresa Empresa { get; set; } = null!;
    }
}
