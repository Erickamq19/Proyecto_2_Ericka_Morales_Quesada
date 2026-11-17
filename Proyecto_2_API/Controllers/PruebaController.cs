using Microsoft.AspNetCore.Mvc;
using Proyecto_2_API.DB;
using Microsoft.EntityFrameworkCore;


namespace Proyecto_2_API.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class PruebaController : ControllerBase
    {
        private readonly conexion_DB _context;

        public PruebaController(conexion_DB context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ProbarConexion()
        {
            bool conectado = await _context.Database.CanConnectAsync();

            if (conectado)
            {
                return Ok("Conexión exitosa con Proyecto_2_DB.");
            }

            return StatusCode(503, "No se pudo conectar con SQL Server.");
        }


    }
}
