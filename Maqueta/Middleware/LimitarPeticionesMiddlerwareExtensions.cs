using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Dtos;
using Maqueta.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Maqueta.Middleware
{
    public static class LimitarPeticionesMiddlerwareExtensions
    {
        public static IApplicationBuilder useLimitarPeticiones(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LimitarPeticionesMiddlerware>();
        }
    }
    public class LimitarPeticionesMiddlerware
    {
        private readonly RequestDelegate siguiente;
        private readonly IConfiguration configuration;

        public LimitarPeticionesMiddlerware(RequestDelegate siguiente, IConfiguration configuration)
        {
            this.siguiente = siguiente;
            this.configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext applicationDbContext)
        {
            var limitarPeticionesConfiguracion = new LimitarPeticionesConfiguracion();

            configuration.GetSection("limitarPeticiones").Bind(limitarPeticionesConfiguracion);
            // aca vamos a incluir una lista blanca pero lo recomendado es hacer la api de login y generador de llaves aparte,
            // para no tener que usar esta lista blanca y dejar esta api que solo consuma peticiones con llave como este es un ejemplo incluimos la lista blanca para poder
            // hacer peticiones si que tenga que pasar una llave en el login o generador de llaves
            var ruta = httpContext.Request.Path.ToString();
            var estaLaRutaEnListaBlanca = limitarPeticionesConfiguracion.ListaBlancaRutas.Any(x => ruta.Contains(x));
            if (estaLaRutaEnListaBlanca)
            {
                await siguiente(httpContext);
                return;
            }
            var llaveStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (llaveStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe proveer la llave en la cabezera X-Api-Key");
                return;
            }
            if (llaveStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe proveer una sola llave en la cabezera");
                return;
            }
            var llave = llaveStringValues[0];
            var llaveDb = await applicationDbContext.LLavesApi.Include(x =>x.RestriccionesDominio).Include(x=>x.RestriccionesIP).FirstOrDefaultAsync(x => x.Llave == llave);
            if (llaveDb == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La Llave no existe");
                return;
            }
            if (!llaveDb.Activa)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La Llave se encuentra inactiva");
                return;
            }
            if (llaveDb.Tipo == TipoLlave.Gratuita)
            {
                var hoy = DateTime.Today;
                var mañana = hoy.AddDays(1);
                var cantidadDePeticiones = await applicationDbContext.Peticiones.CountAsync(x => x.LlaveId == llaveDb.Id && x.FechaPeticion >= hoy && x.FechaPeticion < mañana);
                if (cantidadDePeticiones >= limitarPeticionesConfiguracion.PeticionesPorDiaGratuito)
                {
                    httpContext.Response.StatusCode = 429;
                    await httpContext.Response.WriteAsync("a superado el limite de peticiones diarias , si desea continuar con las peticiones obtenga una llave premium");
                    return;
                }
            }
            var superaRestricciones = PeticionSuperaAlgunaDeLasRestricciones(llaveDb, httpContext);
            if (!superaRestricciones)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }
            var peticion = new Peticion
            {
                FechaPeticion = DateTime.UtcNow,
                LlaveId = llaveDb.Id,
            };
            applicationDbContext.Add(peticion);
            await applicationDbContext.SaveChangesAsync();
            await siguiente(httpContext);

        }
        private bool PeticionSuperaAlgunaDeLasRestricciones(LlaveApi llaveApi, HttpContext httpContext)
        {
            var hayRestricciones = llaveApi.RestriccionesDominio.Any() || llaveApi.RestriccionesIP.Any();
            if (!hayRestricciones)
            {
                return true;
            }
            var superaDominio = PeticionSuperaRestriccionDominio(llaveApi.RestriccionesDominio, httpContext);
            return superaDominio;

        }
        private bool PeticionSuperaRestriccionDominio(List<RestriccionDominio> restriccionDominios, HttpContext httpContext)
        {
            if (restriccionDominios.Count == 0 || restriccionDominios == null)
            {
                return false;
            }
            var referer = httpContext.Request.Headers["referer"].ToString();
            if (referer == null)
            {
                return false;
            }
            Uri myUri = new Uri(referer);
            string host = myUri.Host;

            var superaRestricciones = restriccionDominios.Any(x => x.Dominio == host);
            return superaRestricciones;


        }
        //private bool PeticionSuperaRestriccionIp(List<RestriccionIP> restriccionIp, HttpContext httpContext)
        //{
        //}
    }
}
    
