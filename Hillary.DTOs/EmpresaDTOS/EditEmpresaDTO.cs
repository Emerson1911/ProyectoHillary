namespace Hillary.DTOs.EmpresaDTOS
{
    public class EditEmpresaDTO
    {
        public string? Nombre { get; set; }
        public string? Ruc { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool? Activo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
