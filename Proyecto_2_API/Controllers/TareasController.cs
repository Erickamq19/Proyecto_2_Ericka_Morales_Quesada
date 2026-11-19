using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_2_API.DB;
using Proyecto_2_API.Modelos;
using System.Security.Claims;
using System.Threading;

namespace Proyecto_2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly conexion_DB _context;
        public TareasController(conexion_DB context)
        {
            _context = context;
        }

        private int Obtener_Usuario()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private bool Verif_Rol()
        {
            return User.IsInRole("Administrador");

        }

        private async Task<bool> G_Tareas(Tareas tareas)
        {

            if (Verif_Rol()) 
            { 
                return true;
            }

            int idusuario = Obtener_Usuario();
            var proyecto = await _context.Proyectos.FirstOrDefaultAsync(o => o.Id == tareas.IdProyecto && o.Activo);
            if (proyecto != null) 
            { 
                return true;
            
            }

            return tareas.IdUsuarioAsignado == idusuario || tareas.IdUsuarioCreador == idusuario || proyecto.IdUsuarioCreador == idusuario || proyecto.IdResponsable == idusuario; 


        }

        //Tareas registrados
        [HttpGet]
        public async Task<IActionResult> Tareas_Registradas()
        {
            int idusuario = Obtener_Usuario();
            var consultar = _context.Tareas.AsQueryable();
            if (!Verif_Rol())
            {
                consultar = consultar.Where(l => l.IdUsuarioAsignado == idusuario || l.IdUsuarioCreador == idusuario || _context.Proyectos.Any(h => h.Id == l.IdProyecto
                && h.Activo && (h.IdUsuarioCreador == idusuario || h.IdResponsable == idusuario)));

            }

            var tareas = await consultar.OrderByDescending(l => l.FechaCreacion).ToListAsync();

            return Ok(tareas);
        }

        //Consultar tareas
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscador_P(int id)
        {
            var tarea = await _context.Tareas.FirstOrDefaultAsync(o => o.Id == id);
            if (tarea == null)
            {
                return NotFound("La tarea no existe");

            }

            
            if (!await G_Tareas(tarea) )
            {
                return Forbid();
            }

            return Ok(tarea);
        }

        //Registrar un nueva tarea
        [HttpPost]
        public async Task<IActionResult> Registrar_Proyecto([FromBody] Tareas tarea_S)
        {
            if (proyecto_S.FechaFin.HasValue && proyecto_S.FechaFin.Value.Date < proyecto_S.FechaInicio.Date)
            {
                return BadRequest("Ingrese la fecha corretamente");

            }

            bool verf_resposable = await _context.Usuarios.AnyAsync(u => u.Id == proyecto_S.IdResponsable && u.Activo);
            if (!verf_resposable)
            {
                return BadRequest("El responsable no existe o esta inactivo");

            }

            bool verf_estado = await _context.Estados.AnyAsync(i => i.Id == proyecto_S.IdEstado);
            if (!verf_estado)
            {
                return BadRequest("Error al seleccionar el estado");

            }

            var nuevo_proyecto = new Proyectos
            {
                Nombre = proyecto_S.Nombre,
                Descripcion = proyecto_S.Descripcion,
                FechaInicio = proyecto_S.FechaInicio,
                FechaFin = proyecto_S.FechaFin,
                IdUsuarioCreador = Obtener_Usuario(),
                IdResponsable = proyecto_S.IdResponsable,
                IdEstado = proyecto_S.IdEstado,
                FechaCreacion = DateTime.Now,
                Activo = true
            };

            _context.Proyectos.Add(nuevo_proyecto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(string.Empty, nuevo_proyecto);

        }


    }
}
