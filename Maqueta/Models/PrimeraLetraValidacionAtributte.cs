using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Maqueta.Models
{
    public class PrimeraLetraValidacionAtributte: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) {
                return ValidationResult.Success;
                    }
            var primeraletra = value.ToString()[0].ToString();
            if(primeraletra != primeraletra.ToUpper())
            {
                return new ValidationResult("Debe tener mayuscula en la primera letra");
            }
            return ValidationResult.Success;

        }
    }
}
