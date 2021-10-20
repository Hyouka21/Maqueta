using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Maqueta.Dtos;
using Maqueta.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Maqueta.Controllers
{
    [Route("api/llavesapi")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LlavesApiController :CustomBaseController
    {
        private readonly ApplicationDbContext applicationDb;
        private readonly IMapper mapper;
        private readonly ServicioLlaves servicioLlaves;

        public LlavesApiController(ApplicationDbContext applicationDb,IMapper mapper,ServicioLlaves servicioLlaves)
        {
            this.applicationDb = applicationDb;
            this.mapper = mapper;
            this.servicioLlaves = servicioLlaves;
        }
        [HttpGet]
        public async Task<List<LlaveDto>> misLlaves()
        {
            var usuarioId = obtenerUsuarioId();
            var llaves = await applicationDb.LLavesApi.Where(x => x.UsuarioId == usuarioId).ToListAsync();
            return mapper.Map<List<LlaveDto>>(llaves);
        }
        [HttpPost]
        public async Task<ActionResult> crearLlave(CrearLlaveDto crearLlaveDto)
        {
            var usuarioId = obtenerUsuarioId();
            if(crearLlaveDto.Tipo == Models.TipoLlave.Gratuita)
            {
                var verificaLlaveGratuita = await applicationDb.LLavesApi.AnyAsync(
                    x => x.Tipo == Models.TipoLlave.Gratuita && x.UsuarioId == usuarioId
                    );
                if (verificaLlaveGratuita)
                {
                    return BadRequest("El usuario ya tiene una llave gratuita");
                }
            
            }
            await servicioLlaves.CrearLlave(usuarioId, crearLlaveDto.Tipo);
            return NoContent();

        }
        [HttpPut]
        public async Task<ActionResult> actualizarLlave(ActualizarLlaveDto actualizarLlaveDto)
        {
            var usuarioId = obtenerUsuarioId();
            var LlaveDb = await applicationDb.LLavesApi.FirstOrDefaultAsync(x => x.Id == actualizarLlaveDto.LlaveId);
            if (LlaveDb == null)
            {
                return NotFound();
            }
            if (LlaveDb.UsuarioId != usuarioId)
            {
                return Forbid();
            }
            if (actualizarLlaveDto.ActualizarLlave)
            {
                LlaveDb.Llave = servicioLlaves.generarLlave();
            }
            LlaveDb.Activa = actualizarLlaveDto.Activa;

            await applicationDb.SaveChangesAsync();
            return NoContent();


        }
    }
}
