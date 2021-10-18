using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Models;

namespace Maqueta.Dtos
{
    public class LibroPatchDto
    {
        [PrimeraLetraValidacionAtributte]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
 
    }
}
