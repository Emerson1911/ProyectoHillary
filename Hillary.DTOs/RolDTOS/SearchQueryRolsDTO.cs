namespace Hillary.DTOs.RolDTOS
{
    public class SearchQueryRolsDTO
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
