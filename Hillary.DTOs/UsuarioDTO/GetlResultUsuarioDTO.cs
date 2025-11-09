namespace Hillary.DTOs.UsuarioDTO
{
    public class GetlResultUsuarioDTO
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string? EmpresaNombre { get; set; }
        public int RolId { get; set; }
        public string? RolNombre { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public bool? Activo { get; set; }
    }
}
