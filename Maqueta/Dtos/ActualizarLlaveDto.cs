using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Dtos
{
    public class ActualizarLlaveDto
    {
        public int LlaveId { get; set; }
        public bool ActualizarLlave { get; set; }
        public bool Activa { get; set; }
    }
}
