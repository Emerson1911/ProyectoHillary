namespace Hillary.DTOs.RolDTOS
{
    public class GetIdResultRolsDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int UsuariosCount { get; set; }
    }
}
