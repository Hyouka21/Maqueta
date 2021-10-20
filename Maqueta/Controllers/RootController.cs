using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Maqueta.Controllers
{
    [Route("api/root")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name ="obtenerRoot")]
        public ActionResult<IEnumerable<DatoHATEOS>> Get()
        {
            var datosHateos = new List<DatoHATEOS>();
            datosHateos.Add(new DatoHATEOS(enlace: Url.Link("obtenerRoot",new { }), descripcion: "self", metodo: "GET"));


            return datosHateos;
        }
    }
}
