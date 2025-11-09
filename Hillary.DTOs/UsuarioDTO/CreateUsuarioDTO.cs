namespace Hillary.DTOs.UsuarioDTO
{
    public class CreateUsuarioDTO
    {
        public int EmpresaId { get; set; }
        public int RolId { get; set; }
        public string? Nombre { get; set; }
        //public string? Email { get; set; }
        public string? Password { get; set; }
        //public bool? Activo { get; set; }
    }
}
