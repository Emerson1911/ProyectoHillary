using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hillary.DTOs.EmpresaDTOS
{
    public class SearchQueryEmpresaDTO
    {
        public string? Nombre { get; set; }
        public string? Ruc { get; set; }
        public string? Email { get; set; }
        public bool? Activo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
