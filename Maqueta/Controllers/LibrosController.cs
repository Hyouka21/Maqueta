using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maqueta.Controllers
{
	[ApiController]
	[Route("api/libros")]
	public class LibrosController : ControllerBase
	{
		private readonly ApplicationDbContext applicationDb;

		public LibrosController(ApplicationDbContext applicationDb)
		{
			this.applicationDb = applicationDb;
		}
		[HttpGet("{id:int}")]
		public async Task<ActionResult<Libro>> Get(int id) {

			return await applicationDb.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
		}
		[HttpPost]
		public async Task<ActionResult> Post([FromForm] Libro libro) {
			var existe = await applicationDb.Autores.AnyAsync(x => x.Id == libro.AutorId);
			if (!existe)
			{
				return BadRequest($"No existe el autor con id: {libro.AutorId}");
			}
			applicationDb.Add(libro);
			await applicationDb.SaveChangesAsync();
			return Ok();

		}
	}
}
