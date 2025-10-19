using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hillary.DTOs.UsuarioDTO
{
    public class EditUsuarioDTO
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
       // public bool? Activo { get; set; }
    }
}
