using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Dtos
{
    public class LibrosDtosConAutor : LibroDtos
    {
        public List<AutorDtos> Autores { get; set; }
    }
}
