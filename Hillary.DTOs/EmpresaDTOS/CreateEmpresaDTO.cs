using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hillary.DTOs.EmpresaDTOS
{
    public class CreateEmpresaDTO
    {
        public string? Nombre { get; set; }
        public string? Ruc { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }
}
