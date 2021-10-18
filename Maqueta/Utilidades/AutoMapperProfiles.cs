using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Maqueta.Dtos;
using Maqueta.Models;

namespace Maqueta.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<LibroPatchDto, Libro>().ReverseMap();
            CreateMap<AutorCreacionDtos, Autor>();
            CreateMap<Autor, AutorDtos>();
            CreateMap<Autor, AutorDtosConLibro>().ForMember(autor => autor.Libros, opciones => opciones.MapFrom(MapAutorDtoLibros));
            CreateMap<Libro, LibroDtos>();
            CreateMap<Libro, LibrosDtosConAutor>().ForMember(libro => libro.Autores, opciones => opciones.MapFrom(MapLibroDtoAutores));
            CreateMap<LibroCreacionDtos, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            
        }
        private List<LibroDtos> MapAutorDtoLibros(Autor autor,AutorDtos autordto)
        {
            var resultado = new List<LibroDtos>();
            if (autor.AutoresLibros == null) { return resultado; }
            foreach (var autorlibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDtos()
                {
                    Id = autorlibro.LibroId,
                    Titulo = autorlibro.Libro.Titulo
                });

            }
            return resultado;
        }
        private List<AutorLibro> MapAutoresLibros(LibroCreacionDtos libroCreacion,Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if(libroCreacion.AutoresIds == null) { return resultado; }
            foreach(var autor  in libroCreacion.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autor });
            }
            return resultado;
        }
        private List<AutorDtos> MapLibroDtoAutores(Libro libro,LibroDtos libroDtos)
        {
            var resultado = new List<AutorDtos>();
            if(libro.AutoresLibros == null) { return resultado; }
            foreach (var autorlibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDtos()
                {
                    Id = autorlibro.AutorId,
                    Nombre = autorlibro.Autor.Nombre
                });

            }
            return resultado;

        }
    }
}
