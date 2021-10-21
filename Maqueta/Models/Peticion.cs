using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Models
{
    public class Peticion
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public DateTime FechaPeticion { get; set; }
        public LlaveApi Llave { get; set; }

    }
}
