using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Maqueta.Dtos;
using Maqueta.Filtros;
using Maqueta.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maqueta.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AutoresController : ControllerBase
	{
		private readonly ApplicationDbContext applicationDbContext;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext applicationDbContext, IMapper mapper) {
			this.applicationDbContext = applicationDbContext;
            this.mapper = mapper;
        }
		//[ResponseCache(Duration =10)]
		[HttpGet] // api/autores
		[HttpGet("listado")]// api/autores/listado
		[HttpGet("/listado")] // listado
		[ServiceFilter(typeof(MiFiltroDeAccion))]
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="EsAdmin")]
		public async Task<ActionResult<List<Autor>>> Get() {
			return await applicationDbContext.Autores.ToListAsync();

		}
		[HttpGet("primero")]
		public async Task<ActionResult<Autor>> Primero([FromHeader] string numero, [FromQuery] string nombre) {
			return await applicationDbContext.Autores.FirstOrDefaultAsync();
		}
		[HttpGet("{id:int}",Name ="ObtenerAutor")]
		public async Task<ActionResult<AutorDtosConLibro>> Get(int id) {
			var autor =  await applicationDbContext.Autores
				.Include(x=>x.AutoresLibros)
				.ThenInclude(x=>x.Libro)
				.FirstOrDefaultAsync(x => x.Id == id);
			if (autor == null) {

				return NotFound();
			}
			return mapper.Map<AutorDtosConLibro>(autor);
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
			var existeAutorConElMismoNombre = await applicationDbContext.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            if (existeAutorConElMismoNombre)
            {
				return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }
			applicationDbContext.Add(autor);
			await applicationDbContext.SaveChangesAsync();
			var autorDto = mapper.Map<AutorDtos>(autor);
			return CreatedAtRoute("ObtenerAutor",new { Id = autor.Id},autorDto);
		}
		[HttpPut("{id:int}")]//api/autores/1
		public async Task<ActionResult> Put([FromForm] AutorCreacionDtos autorDtos, int id) {
			var existe = await applicationDbContext.Autores.AnyAsync(x => x.Id == id);
			if (!existe)
			{
				return NotFound();
			}
			var autor = mapper.Map<Autor>(autorDtos);
			applicationDbContext.Update(autor);
			await applicationDbContext.SaveChangesAsync();
			return NoContent();

		}
		[HttpDelete("{id:int}")]
		public async Task<ActionResult> Delete(int id){
			var existe = await applicationDbContext.Autores.AnyAsync(x => x.Id == id);
			if (!existe) {
				return NotFound();
			}
			applicationDbContext.Remove(new Autor() { Id = id });
			await applicationDbContext.SaveChangesAsync();
			return NoContent();
		}
		
	}
}
