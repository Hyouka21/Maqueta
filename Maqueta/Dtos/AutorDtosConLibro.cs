using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Dtos
{
    public class AutorDtosConLibro:AutorDtos
    {
        public List<LibroDtos> Libros { get; set; }
    }
}
