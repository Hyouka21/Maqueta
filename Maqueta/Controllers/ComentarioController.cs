using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Maqueta.Controllers
{
    [Route("api/libro/{libroId:int}/comentarios")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ComentariosController(ApplicationDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async  Task<ActionResult<List<Comentario>>> Get(int libroId)
        {
            var comentarios = await context.Comentarios.Where(x => x.LibroId == libroId).ToListAsync();
            return Ok(comentarios);

        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, [FromForm] string contenido)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existe)
            {
                return BadRequest($"No existe el libro con id: {libroId}");
            }
            var comen = new Comentario()
            {
                LibroId = libroId,
                Contenido = contenido
            };
            context.Add(comen);
            await context.SaveChangesAsync();
            return Ok();

        }

    }
}
