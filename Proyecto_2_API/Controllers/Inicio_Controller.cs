using Microsoft.AspNetCore.Mvc;
using Proyecto_2_API.Interfaces;
using Proyecto_2_API.Modelos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;


namespace Proyecto_2_API.Controllers
{
    [ApiController]
    [Route("api/Inicio")]
    public class Inicio_Controller : ControllerBase
    {
        private readonly Interface_JWT I_JWT;

        public Inicio_Controller(Interface_JWT _JWT) 
        { 
            I_JWT = _JWT;
        
        }

        [HttpPost]
        public async Task<IActionResult> InicioSesion([FromBody] Inicio_U usuario) 
        { 
            var token = await I_JWT.Inicio_S_Async(usuario);
            if (token == null) 
            { 
                return Unauthorized(new { mensaje = "Correo electrónico o contraseña incorrectos"});

            }

            var lector = new JwtSecurityTokenHandler();
            var token_l = lector.ReadJwtToken(token);
            var nombre = token_l.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Name)?.Value;
            return Ok(new { mensaje = $"Bienvenido, {nombre}", token = token });
        }
    }
}
