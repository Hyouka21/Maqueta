using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Maqueta.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Maqueta.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDataProtector dataprotector;


        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtection)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            dataprotector = dataProtection.CreateProtector("valor_unico_y_secreto");

        }
        [HttpGet("encriptar")]
        public ActionResult Encriptar()
        {
            var texto = "Gaston Sosa";
            var textoCifrado = dataprotector.Protect(texto);
            var textoDesencriptado = dataprotector.Unprotect(textoCifrado);
            return Ok(new { textoCifrado, textoDesencriptado });
        }
        [HttpGet("encriptarportiempo")]
        public ActionResult EncriptarPorTiempo()
        {
            //para limitar el tiempo de encriptacion si se pasa del tiempo no puede ser desencriptado
            var protectorLimitado = dataprotector.ToTimeLimitedDataProtector();
            var texto = "Gaston Sosa";
            var textoCifrado = protectorLimitado.Protect(texto,lifetime:TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);//duermo por 6 segundos
            var textoDesencriptado = protectorLimitado.Unprotect(textoCifrado);
            return Ok(new { textoCifrado, textoDesencriptado });
        }
        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutentificacion>> Registrar(CredencialesUsuario credencialUsuario)
        {
            
            var usuario = new IdentityUser { UserName = credencialUsuario.Email, Email = credencialUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario,credencialUsuario.Password);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutentificacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email
                ,credencialesUsuario.Password,isPersistent:false,lockoutOnFailure:false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("login incorrecto");
            }
        }
        [HttpGet("renovartoken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public  async Task<ActionResult<RespuestaAutentificacion>> RenovarToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
       
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email=email,
            };
            return await ConstruirToken(credencialesUsuario);

        }
        private async Task<RespuestaAutentificacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claim = new List<Claim>()
            {
                new Claim("email",credencialesUsuario.Email)
            };
            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimBd = await userManager.GetClaimsAsync(usuario);
            claim.AddRange(claimBd);
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claim, expires: expiracion, signingCredentials: creds);
            return new RespuestaAutentificacion
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
         
        }
        [HttpPost("haceradmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDto editarAdmin)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdmin.Email);
            await userManager.AddClaimAsync(usuario, new Claim("EsAdmin", "1"));
            return NoContent();

        }
        [HttpPost("removeradmin")]
        public async Task<ActionResult> removerAdmin(EditarAdminDto editarAdmin)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdmin.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("EsAdmin", "1"));
            return NoContent();

        }
    }
}
