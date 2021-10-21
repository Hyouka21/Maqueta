using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Maqueta.Models
{
    public class LlaveApi
    {
        public int Id { get; set; }
        public string Llave { get; set; }
        public string UsuarioId { get; set; }
        public TipoLlave Tipo { get; set; }
        public bool Activa { get; set; }
        public IdentityUser Usuario { get; set; }
        public List<RestriccionDominio> RestriccionesDominio { get; set; }
        public List<RestriccionIP> RestriccionesIP { get; set; }

    }
    public enum TipoLlave
    {
        Gratuita=1,
        Profesional=2
    }
}
