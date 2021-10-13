using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Maqueta.Servicios
{/// <summary>
/// Crear un servicio que escriba en un archivo y ademas tenga una funcionalidad que cada 5 segundo se ejecute y escriba en el archivo
/// </summary>
    public class EscribirEnArchivo : IHostedService
    {
        private Timer timer;
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo 1.txt";

        public EscribirEnArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
           ///para ejecutar una funcion cada 5 seg timer = new Timer(DoWork,null, TimeSpan.Zero,TimeSpan.FromSeconds(5));
            Escribir("Proceso Iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //timer.Dispose();
            Escribir("Proceso Finalizado");
            return Task.CompletedTask;
        }
        private void DoWork(object state)
        {
            Escribir("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }
        private void Escribir (string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}
