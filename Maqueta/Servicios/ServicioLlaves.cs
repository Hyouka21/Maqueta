using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maqueta.Models;

namespace Maqueta.Servicios
{
    public class ServicioLlaves
    {
        private readonly ApplicationDbContext applicationDb;

        public ServicioLlaves(ApplicationDbContext applicationDb)
        {
            this.applicationDb = applicationDb;
        }
        public async Task CrearLlave(string usuarioId , TipoLlave tipo)
        {
            var llave = generarLlave();
            var llaveApi = new LlaveApi
            {
                Activa = true,
                Llave = llave,
                UsuarioId = usuarioId,
                Tipo = tipo
            };
            applicationDb.Add(llaveApi);
            await applicationDb.SaveChangesAsync();

        }
        public string generarLlave()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
