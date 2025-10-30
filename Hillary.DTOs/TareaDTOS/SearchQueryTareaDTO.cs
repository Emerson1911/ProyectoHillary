using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hillary.DTOs.TareaDTOS
{
    public class SearchQueryTareaDTO
    {
        public string? Nombre { get; set; }
        // EmpresaId se filtra automáticamente del token
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
