using Microsoft.AspNetCore.Mvc;
using Proyecto_2_API.Interfaces;
using Proyecto_2_API.Modelos;


namespace Proyecto_2_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class Registro_U_Controller : ControllerBase
    {
        private readonly Interface_U _U;

        public Registro_U_Controller (Interface_U u)
        {
            _U = u;
        }

        [HttpPost]
         public async Task<IActionResult> Registrar([FromBody] Registro_U registro_U) 
        { 
            var resultado = await _U.RegistroAsync(registro_U);

            if (!resultado.Exito) 
            { 
                return BadRequest(new { mensaje = resultado.Mensaje});
            
            }

            return Ok(new {mensaje = resultado.Mensaje});
        
        }
    }
}
