using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_2_API.DB;
using Proyecto_2_API.Modelos;

namespace Proyecto_2_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : ControllerBase
    {
        private readonly conexion_DB _context;
        public UsuarioController(conexion_DB context)
        {
            _context = context;
        }


        //Uusuario registrados
        [HttpGet]
        public async Task<IActionResult> Usuarios_Registrados()
        {
            var usuarios = await _context.Usuarios.Select(u => new 
            { 
                u.Id,
                u.Nombre,
                u.Email,
                u.IdRol,
                u.FechaRegistro,
                u.Activo
            }).OrderBy(u => u.Nombre).ToListAsync();

            return Ok(usuarios);
        }

        //Consultar Proyectos
        [HttpGet("{id}")]
        public async Task<IActionResult> Consultar_U(int id)
        {

            var usuarios = await _context.Usuarios.Where(u => u.Id == id).Select(u => new
            {
                u.Id,
                u.Nombre,
                u.Email,
                u.IdRol,
                Rol = u.IdRol == 1? "Administrador" : "Usuario",
                u.FechaRegistro,
                u.Activo
            }).FirstOrDefaultAsync();

            if (usuarios == null) 
            {
                return NotFound("El usuario no existe");
            }
            return Ok(usuarios);
        }


        //Actualizar un proyecto
        [HttpPut("{id}")]
        public async Task<IActionResult> Actu_Usuario(int id, [FromBody] Usuarios usuario_S)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(o => o.Id == id);
            if (usuario == null)
            {
                return NotFound("El usuario no existe");

            }

            bool verif_email = await _context.Usuarios.AnyAsync(o => o.Email == usuario_S.Email && o.Id != id);


            if (verif_email)
            {
                return BadRequest("El correo eletrónico ya se encuentra registrado");

            }

            bool verif_rol = await _context.Roles.AnyAsync(p => p.Id == usuario_S.IdRol);

            if (!verif_rol)
            {
                return BadRequest("El rol no existe");

            }

            

            usuario.Nombre = usuario_S.Nombre;
            usuario.Email = usuario_S.Email;
            usuario.IdRol = usuario_S.IdRol;
            usuario.Activo = usuario_S.Activo;
           


            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "El usuario se actualizó corretamente",
                usuario.Id,
                usuario.Nombre,
                usuario.Email,  
                usuario.IdRol,
                usuario.Activo,
            });

        }


        //Eliminar un proyecto
        [HttpDelete("{id}")]
        public async Task<IActionResult> Desc_Usuario(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(o => o.Id == id && o.Activo);
            if (usuario == null)
            {
                return NotFound("El usuario no existe");

            }
            bool proyecto_r = await _context.Proyectos.AnyAsync(o => o.IdUsuarioCreador == id || o.IdResponsable == id);
            bool tarea_r = await _context.Tareas.AnyAsync(o => o.IdUsuarioCreador == id || o.IdUsuarioAsignado == id);

            if (proyecto_r || tarea_r) 
            {
                return BadRequest("El usuario tiene proyectos o tareas relacionadas");
            
            }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new {mensaje = "Usuario eliminado correctamente"});



        }
    }
}
