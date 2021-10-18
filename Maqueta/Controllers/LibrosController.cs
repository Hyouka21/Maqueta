using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Maqueta.Dtos;
using Maqueta.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maqueta.Controllers
{
	[ApiController]
	[Route("api/libros")]
	public class LibrosController : ControllerBase
	{
		private readonly ApplicationDbContext applicationDb;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext applicationDb, IMapper mapper)
		{
			this.applicationDb = applicationDb;
            this.mapper = mapper;
        }
		[HttpGet("{id:int}", Name = "ObtenerLibro")]
		public async Task<ActionResult<LibrosDtosConAutor>> Get(int id) {

			var libros=await applicationDb.Libros.Include(x => x.Comentarios)
				.Include(x=> x.AutoresLibros).ThenInclude(x=>x.Autor)
				.FirstOrDefaultAsync(x => x.Id == id);
			libros.AutoresLibros = libros.AutoresLibros.OrderBy(x => x.Orden).ToList();
			return mapper.Map<LibrosDtosConAutor>(libros);
		}
		[HttpPost]
		public async Task<ActionResult> Post([FromForm] LibroCreacionDtos libroCreacion) {
			if (libroCreacion.AutoresIds == null)
			{
				return BadRequest("No se puede crear un libro sin autores");
			}
			var autoresIds = await applicationDb.Autores
				.Where(autor => libroCreacion.AutoresIds.Contains(autor.Id)).Select(x => x.Id).ToListAsync();
			if(libroCreacion.AutoresIds.Count != autoresIds.Count)
            {
				return BadRequest("No existe uno de los autores enviados");
            }
			var libro = mapper.Map<Libro>(libroCreacion);
			asignarOrdenAutores(libro);
			applicationDb.Add(libro);
			await applicationDb.SaveChangesAsync();
			var libroDto = mapper.Map<LibroDtos>(libro);
			return CreatedAtRoute("ObtenerLibro", new { Id = libro.Id }, libroDto);

		}
		[HttpPut("{id:int}")]
		public async Task<ActionResult> Put(int id,[FromForm]LibroCreacionDtos libroCreacionDtos)
        {
			var libroDb = await applicationDb.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(x => x.Id == id);
			if (libroDb == null)
            {
				return NotFound();
            }
			libroDb = mapper.Map(libroCreacionDtos, libroDb);
			asignarOrdenAutores(libroDb);
			await applicationDb.SaveChangesAsync();
			return NoContent();

        }
		private void asignarOrdenAutores(Libro libro)
        {
			if (libro.AutoresLibros != null)
			{
				for (int i = 0; i < libro.AutoresLibros.Count; i++)
				{
					libro.AutoresLibros[i].Orden = i;

				}
			}
		}
		[HttpPatch("{id:int}")]
		public async Task<ActionResult> Patch(int id, [FromForm] JsonPatchDocument<LibroPatchDto> jsonPatch)
        {
			if(jsonPatch == null)
            {
				return BadRequest();
            }
			var libroDb = applicationDb.Libros.FirstOrDefaultAsync(x => x.Id == id);
            if (libroDb == null)
            {
				return NotFound();
            }
			var libroDto = mapper.Map<LibroPatchDto>(libroDb);

			jsonPatch.ApplyTo(libroDto, ModelState);
			var esValido = TryValidateModel(libroDto);
            if (!esValido)
            {
				return BadRequest(ModelState);
            }
			await mapper.Map(libroDto,libroDb);
			await applicationDb.SaveChangesAsync();
			return NoContent();

        }
	}
}
