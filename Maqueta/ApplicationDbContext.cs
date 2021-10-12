using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Models;
using Microsoft.EntityFrameworkCore;

namespace Maqueta
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext([NotNullAttribute] DbContextOptions options) : base(options)
		{
		}
		public DbSet<Autor> Autores { get; set; }
		public DbSet<Libro> Libros { get; set; }
	}
}
