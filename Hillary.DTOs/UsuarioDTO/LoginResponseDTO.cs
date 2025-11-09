namespace Hillary.DTOs.UsuarioDTO
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int EmpresaId { get; set; }
        public string EmpresaNombre { get; set; } = string.Empty;
        public int RolId { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
