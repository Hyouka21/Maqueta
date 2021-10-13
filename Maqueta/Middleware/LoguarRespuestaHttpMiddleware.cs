using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Maqueta.Middleware
{
    public static class LoguarRespuestaHttpMiddlewareExtensions
    {
        public static IApplicationBuilder useLoguearRespuestaHttp(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguarRespuestaHttpMiddleware>();
        }
    }
    public class LoguarRespuestaHttpMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguarRespuestaHttpMiddleware> logger;

        public LoguarRespuestaHttpMiddleware(RequestDelegate siguiente, ILogger<LoguarRespuestaHttpMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

      
        // Invoke o Invoke async
        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;
                await siguiente(contexto);
                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;
                logger.LogInformation(respuesta);
            }
        }
    }
}
