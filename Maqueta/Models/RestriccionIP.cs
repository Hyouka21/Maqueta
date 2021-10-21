using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Models
{
    public class RestriccionIP
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        // se podria restringir por rango de ip tambien en este caso solo lo vamos a hacer por una ip determinada , pero en un futuro se podria restringir por rango 
        public string IP { get; set; }
        public LlaveApi Llave { get; set; }
    }
}
