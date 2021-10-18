using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Models;

namespace Maqueta.Dtos
{
    public class LibroDtos
    {
        public int Id { get; set; }
        [PrimeraLetraValidacionAtributte]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        //public List<Comentario> Comentarios { get; set; }


    }
}
