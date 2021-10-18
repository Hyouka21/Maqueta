using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Models
{
	public class Autor : IValidatableObject
	{
		public int Id { get; set; }
		[Required(ErrorMessage ="El campo {0} es obligatorio")]// {0} sustituye el valor por el nombre de la propiedad
		[StringLength(maximumLength:50,ErrorMessage ="El campo {0} no puede ser mayor a {1} caracteres")]
		[PrimeraLetraValidacionAtributte]
		public string Nombre { get; set; }
        //[Range(18,50,ErrorMessage ="El campo {0} debe ser mayor a {1} y menor a {2}")]
        //[NotMapped]
        //      public int Édad { get; set; }
        //[CreditCard]
        //[NotMapped]
        //public string TarjetaDeCredito { get; set; }
        //[Url]
        //[NotMapped]
        //      public string UrlFoto { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraletra = Nombre[0].ToString();
                if (primeraletra != primeraletra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula", new string[] { nameof(Nombre) });
                }
            }
           
        }
    }
}
