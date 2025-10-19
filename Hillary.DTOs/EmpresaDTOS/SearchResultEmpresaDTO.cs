using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hillary.DTOs.EmpresaDTOS
{
    public class SearchResultEmpresaDTO
    {
        public int CountRow { get; set; }
        public List<EmpresaDTO> Data { get; set; } = new();

        public class EmpresaDTO
        {
            public int Id { get; set; }
            public string? Nombre { get; set; }
            public string? Ruc { get; set; }
            public string? Direccion { get; set; }
            public string? Telefono { get; set; }
            public string? Email { get; set; }
            public bool? Activo { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public int TareasCount { get; set; }
            public int UsuariosCount { get; set; }
        }
    }
}
