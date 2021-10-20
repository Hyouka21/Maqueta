using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Maqueta.Controllers
{
    public class CustomBaseController:ControllerBase
    {
        protected string obtenerUsuarioId()
        {
            var usuarioClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            return usuarioClaim.Value;
        }
    }
}
