using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Proyecto_2_API.DB;
using Proyecto_2_API.Modelos;
namespace Proyecto_2_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProyectosController : ControllerBase
    {
        private readonly conexion_DB _context;
        public ProyectosController(conexion_DB context)
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

        private bool G_Proyectos(Proyectos proyectos)
        {
            return Verif_Rol() || proyectos.IdUsuarioCreador == Obtener_Usuario() || proyectos.IdResponsable == Obtener_Usuario();


        }


        //Proyectos registrados
        [HttpGet]
        public async Task<IActionResult> Proyecto_Registrados()
        {
            int idusuario = Obtener_Usuario();
            var consultar = _context.Proyectos.Where(l => l.Activo);
            if (!Verif_Rol())
            {
                consultar = consultar.Where(l => l.IdUsuarioCreador == idusuario || l.IdResponsable == idusuario);

            }

            var proyectos = await consultar.OrderByDescending(l => l.FechaCreacion).ToListAsync();

            return Ok(proyectos);
        }

        //Consultar Proyectos
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscador_P(int id)
        {


            var proyecto = await _context.Proyectos.Where(u => u.Id == id && u.Activo).Select(u => new
            {
                u.Id,
                u.Nombre,
                u.Descripcion,
                u.FechaInicio,
                u.FechaFin,
                u.IdUsuarioCreador, Creador = _context.Usuarios.Where(j => j.Id == u.IdUsuarioCreador).Select(u => u.Nombre).FirstOrDefault(),
                u.IdResponsable, Responsable = _context.Usuarios.Where(j => j.Id == u.IdResponsable).Select(u => u.Nombre).FirstOrDefault(),
                u.IdEstado, Estado = _context.Estados.Where(j => j.Id == u.IdEstado).Select(u => u.Nombre).FirstOrDefault(),
                u.FechaCreacion,
                u.Activo
            }).FirstOrDefaultAsync();

            
            if (proyecto == null) 
            {
                return NotFound("No se encontraron proyectos relacionados");
               
            }

            var proyecto_v = await _context.Proyectos.FirstOrDefaultAsync(u => u.Id == id);

            if (!G_Proyectos(proyecto_v!)) 
            {
                return Forbid();
            
            }

            return Ok(proyecto);
            

            
        }

        //Registrar un nuevo proyecto
        [HttpPost]
        public async Task<IActionResult> Registrar_Proyecto([FromBody] Proyectos proyecto_S)
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
            return Ok(new { 

                mensaje = "Proyecto registrado",
                nuevo_proyecto.Id,
                nuevo_proyecto.Nombre,
                nuevo_proyecto.IdUsuarioCreador,
                nuevo_proyecto.IdResponsable,
                nuevo_proyecto.IdEstado
            });

        }

        //Actualizar un proyecto
        [HttpPut("{id}")]
        public async Task<IActionResult> Actu_Proyecto( int id, [FromBody] Proyectos proyecto_S)
        {
            var proyecto = await _context.Proyectos.FirstOrDefaultAsync(o => o.Id == id && o.Activo);
            if (proyecto == null) 
            {
                return NotFound("El proyecto no existe");
            
            }

            if (!G_Proyectos(proyecto)) 
            {
                return Forbid();
            
            }

          
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

            proyecto.Nombre = proyecto_S.Nombre;
            proyecto.Descripcion = proyecto_S.Descripcion;
            proyecto.FechaInicio = proyecto_S.FechaInicio;
            proyecto.FechaFin = proyecto_S.FechaFin;
            proyecto.IdResponsable = proyecto_S.IdResponsable;
            proyecto.IdEstado = proyecto_S.IdEstado;


            await _context.SaveChangesAsync();
            return Ok(proyecto);

        }


        //Eliminar un proyecto
        [HttpDelete("{id}")]
        public async Task<IActionResult> Desc_Proyecto(int id)
        {
            var proyecto = await _context.Proyectos.FirstOrDefaultAsync(o => o.Id == id && o.Activo);
            if (proyecto == null)
            {
                return NotFound("El proyecto no existe");

            }

            if (!G_Proyectos(proyecto))
            {
                return Forbid();

            }
             bool t_relacionada = await _context.Tareas.AnyAsync(o => o.IdProyecto == id);
            if (t_relacionada) {

                return BadRequest("El proyecto tiene por lo menos una tarea relacionada");
            }
            _context.Proyectos.Remove(proyecto);
            await _context.SaveChangesAsync();

            return Ok(new {mensaje = "Proyecto eliminado"});

          

        }


    }
}
