using System.ComponentModel.DataAnnotations;
using Maqueta.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaquetaTest.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraValidacionAtributteTest
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraValidacionAtributte();
            var valor = "gaston";
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificación
            Assert.AreEqual("Debe tener mayuscula en la primera letra", resultado.ErrorMessage);
        }
        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraValidacionAtributte();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificación
            Assert.IsNull(resultado);
        }
        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {
            //Preparación
            var primeraLetraMayuscula = new PrimeraLetraValidacionAtributte();
            string valor = "Gaston";
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecución
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //Verificación
            Assert.IsNull(resultado);
        }
    }
}
