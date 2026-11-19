using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Proyecto_2_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        [HttpGet("Administrador")]
        [Authorize(Roles = "Administrador")]
        public IActionResult AccesoAdministrador()
        {
            return Ok("Acceso permitido. Tu rol es Administrador.");
        }

        [HttpGet("Usuario")]
        [Authorize(Roles = "Usuario")]
        public IActionResult AccesoUsuario()
        {
            return Ok("Acceso permitido. Tu rol es Usuario.");
        }

    }
}
