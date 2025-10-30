using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hillary.DTOs.UsuarioDTO
{
    public class SearchQueryUsuarioDTO
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// public int? EmpresaId { get; set; }
        /// </summary>
        public int? RolId { get; set; }
        public bool? Activo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
