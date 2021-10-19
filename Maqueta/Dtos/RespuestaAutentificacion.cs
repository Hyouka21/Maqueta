using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Dtos
{
    public class RespuestaAutentificacion
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
