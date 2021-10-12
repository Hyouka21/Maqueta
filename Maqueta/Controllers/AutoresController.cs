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
	[Route("api/[controller]")]
	public class AutoresController : ControllerBase
	{
		private readonly ApplicationDbContext applicationDbContext;

		public AutoresController(ApplicationDbContext applicationDbContext) {
			this.applicationDbContext = applicationDbContext;
		}
		[HttpGet] // api/autores
		[HttpGet("listado")]// api/autores/listado
		[HttpGet("/listado")] // listado
		public async Task<ActionResult<List<Autor>>> Get() {
			return await applicationDbContext.Autores.Include(x => x.Libros).ToListAsync();

		}
		[HttpGet("primero")]
		public async Task<ActionResult<Autor>> Primero([FromHeader] string numero, [FromQuery] string nombre) {
			return await applicationDbContext.Autores.FirstOrDefaultAsync();
		}
		[HttpGet("{id:int}")]
		public async Task<ActionResult<Autor>> Get(int id) {
			var autor =  await applicationDbContext.Autores.FirstOrDefaultAsync(x => x.Id == id);
			if (autor == null) {

				return NotFound();
			}
			return Ok(autor);
		}
		[HttpGet("{nombre}")]
		public async Task<ActionResult<Autor>> Get(string nombre)
		{
			var autor = await applicationDbContext.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
			if (autor == null)
			{

				return NotFound();
			}
			return Ok(autor);
		}
		[HttpPost]
		public async Task<ActionResult> Post([FromForm] Autor autor) {
			applicationDbContext.Add(autor);
			await applicationDbContext.SaveChangesAsync();
			return Ok();
		}
		[HttpPut("{id:int}")]//api/autores/1
		public async Task<ActionResult> Put([FromForm] Autor autor, int id) {
			var existe = await applicationDbContext.Autores.AnyAsync(x => x.Id == id);
			if (!existe)
			{
				return NotFound();
			}
			if (autor.Id != id) {
				return BadRequest("El id del autor no coincide con el de la url");
			}
			applicationDbContext.Update(autor);
			await applicationDbContext.SaveChangesAsync();
			return Ok();

		}
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id){
			var existe = await applicationDbContext.Autores.AnyAsync(x => x.Id == id);
			if (!existe) {
				return NotFound();
			}
			applicationDbContext.Remove(new Autor() { Id = id });
			await applicationDbContext.SaveChangesAsync();
			return Ok();
		}
		
	}
}
