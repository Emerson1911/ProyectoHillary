using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hillary.DTOs.TareaDTOS
{
    public class SearchResultTareaDTO
    {
        public int CountRow { get; set; }
        public List<TareaDTO> Data { get; set; } = new List<TareaDTO>();

        public class TareaDTO
        {
            public int Id { get; set; }
            public int EmpresaId { get; set; }
            public string? Nombre { get; set; }
            public string? Descripcion { get; set; }
            public string? EmpresaNombre { get; set; }
        }
    }
}
