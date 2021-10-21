using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Models
{
    public class RestriccionDominio
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public string Dominio { get; set; }
        public LlaveApi Llave { get; set; }
    }
}
