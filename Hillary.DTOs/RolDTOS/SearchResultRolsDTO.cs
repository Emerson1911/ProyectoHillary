namespace Hillary.DTOs.RolDTOS
{
    public class SearchResultRolsDTO
    {
        public int CountRow { get; set; }
        public List<RolsDTO> Data { get; set; } = new();

        public class RolsDTO
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
            public string? Descripcion { get; set; }
            public int UsuariosCount { get; set; }
        }
    }
}
