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
            if (proyecto == null)
            {
                return false;

            }

            return tareas.IdUsuarioAsignado == idusuario || tareas.IdUsuarioCreador == idusuario 
                || proyecto.IdUsuarioCreador == idusuario || proyecto.IdResponsable == idusuario;


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

        //Consultar tarea
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscador_T(int id)
        {


            var tarea = await _context.Tareas.Where(u => u.Id == id).Select(u => new
            {
                u.Id,
                u.Titulo,
                u.Descripcion,
                u.IdProyecto, 
                Proyecto = _context.Proyectos.Where(j => j.Id == u.IdProyecto).Select(u => u.Nombre).FirstOrDefault(),
                u.IdUsuarioCreador,
                Creador = _context.Usuarios.Where(j => j.Id == u.IdUsuarioCreador).Select(u => u.Nombre).FirstOrDefault(),
                u.IdUsuarioAsignado,
                Asignado = _context.Usuarios.Where(j => j.Id == u.IdUsuarioAsignado).Select(u => u.Nombre).FirstOrDefault(),
                u.IdEstado,
                Estado = _context.Estados.Where(j => j.Id == u.IdEstado).Select(u => u.Nombre).FirstOrDefault(),
                u.IdPrioridad,
                Prioridad = _context.Prioridades.Where(j => j.Id == u.IdPrioridad).Select(u => u.Nombre).FirstOrDefault(),
                u.FechaCreacion,
                u.FechaActualizacion,
                u.FechaLimite,
                u.FechaCompletada
            }).FirstOrDefaultAsync();


            if (tarea == null)
            {
                return NotFound("No se encontraron proyectos relacionados");

            }

            var tarea_v = await _context.Tareas.FirstOrDefaultAsync(u => u.Id == id);

            if (!await G_Tareas(tarea_v!))
            {
                return Forbid();

            }

            return Ok(tarea);



        }

        //Registrar un nueva tarea
        [HttpPost]
        public async Task<IActionResult> Registrar_Tarea([FromBody] Tareas tarea_S)
        {
            var proyecto = await _context.Proyectos.FirstOrDefaultAsync(o => o.Id == tarea_S.IdProyecto && o.Activo);

            if (proyecto == null)
            {
                return BadRequest("El proyecto no existe");

            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(o => o.Id == tarea_S.IdUsuarioAsignado && o.Activo);
            if (usuario == null)
            {
                return BadRequest("El usuario no existe");

            }

            if (!Verif_Rol())
            {
                int idusuario = Obtener_Usuario();

                if (proyecto.IdUsuarioCreador != idusuario && proyecto.IdResponsable != idusuario)
                {
                    return Forbid();

                }

            }
            bool verf_estado = await _context.Estados.AnyAsync(i => i.Id == tarea_S.IdEstado);
            if (!verf_estado)
            {
                return BadRequest("Error al seleccionar el estado");

            }

            bool prioridad = await _context.Prioridades.AnyAsync(i => i.Id == tarea_S.IdPrioridad);
            if (!prioridad)
            {
                return BadRequest("Error al seleccionar la prioridad");

            }

            tarea_S.IdUsuarioCreador = Obtener_Usuario();
            tarea_S.FechaCreacion = DateTime.Now;
            tarea_S.FechaActualizacion = DateTime.Now;

            if (tarea_S.IdEstado == 3)
            {
                tarea_S.FechaCompletada = DateTime.Now;
            }

            _context.Tareas.Add(tarea_S);
            await _context.SaveChangesAsync();
            return Ok(new {
                mensaje = "Tarea registrada",
                tarea_S.Id,
                tarea_S.Titulo,
                tarea_S.IdProyecto,
                tarea_S.IdUsuarioAsignado,
                tarea_S.IdEstado,
                tarea_S.IdPrioridad,

            });

        }

        //Actualizar una tarea
        [HttpPut("{id}")]
        public async Task<IActionResult> Actu_Tarea(int id, [FromBody] Tareas tarea_S)
        {

            var tarea = await _context.Tareas.FirstOrDefaultAsync(o => o.Id == id);
            if (tarea == null) 
            {
                return NotFound("La tarea no existe");


            }

            if (!await G_Tareas(tarea)) 
            {
                return Forbid();
            
            }

            var proyecto = await _context.Proyectos.FirstOrDefaultAsync(o => o.Id == tarea_S.IdProyecto && o.Activo);

            if (proyecto == null)
            {
                return BadRequest("El proyecto no existe");

            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(o => o.Id == tarea_S.IdUsuarioAsignado && o.Activo);
            if (usuario == null) 
            {
                return BadRequest("El usuario no existe");
            
            }

            if (!Verif_Rol())
            {
                int idusuario = Obtener_Usuario();

                if (proyecto.IdUsuarioCreador != idusuario && proyecto.IdResponsable != idusuario)
                {
                    return Forbid();

                }

            }

            bool verf_estado = await _context.Estados.AnyAsync(i => i.Id == tarea_S.IdEstado);
            if (!verf_estado)
            {
                return BadRequest("Error al seleccionar el estado");

            }

            bool prioridad = await _context.Prioridades.AnyAsync(i => i.Id == tarea_S.IdPrioridad);
            if (!prioridad)
            {
                return BadRequest("Error al seleccionar la prioridad");

            }

            tarea.Titulo = tarea_S.Titulo;
            tarea.Descripcion = tarea_S.Descripcion;
            tarea.IdProyecto = tarea_S.IdProyecto;
            tarea.IdUsuarioAsignado = tarea_S.IdUsuarioAsignado;
            tarea.IdEstado = tarea_S.IdEstado;
            tarea.IdPrioridad = tarea_S.IdPrioridad;
            tarea.FechaLimite = tarea_S.FechaLimite;
            tarea.FechaActualizacion = DateTime.Now;

            if (tarea.IdEstado == 3 && tarea.FechaCompletada == null)
            {
                tarea.FechaCompletada = DateTime.Now;
            }



            if (tarea.IdEstado != 3 )
            {
                tarea.FechaCompletada = null;
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "La tarea se ha actualizado correctamente",
                tarea.Id,
                tarea.Titulo,
                tarea.IdProyecto,
                tarea.IdUsuarioAsignado,
                tarea.IdEstado,
                tarea.IdPrioridad,

            });

        }


        //Eliminar una tarea
        [HttpDelete("{id}")]
        public async Task<IActionResult> Desc_Tareas(int id)
        {
            var tarea = await _context.Tareas.FirstOrDefaultAsync(o => o.Id == id);
            if (tarea == null)
            {
                return NotFound("La tarea no existe");

            }

            if (!await G_Tareas(tarea))
            {
                return Forbid();

            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return Ok(new {mensaje = "Tarea eliminada"});



        }




    }
}
