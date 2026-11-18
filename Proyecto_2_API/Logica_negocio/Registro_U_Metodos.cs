using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto_2_API.DB;
using Proyecto_2_API.Interfaces;
using Proyecto_2_API.Modelos;

namespace Proyecto_2_API.Logica_negocio

{
    public class Registro_U_Metodos : Interface_U
    {
        private readonly conexion_DB _context;
        private readonly PasswordHasher<Usuarios> _passwordHasher;

        public Registro_U_Metodos(conexion_DB context, PasswordHasher<Usuarios> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Exito, string Mensaje)> RegistroAsync(Registro_U usuario) 
        {
            bool existe = await _context.Usuarios.AnyAsync(i => i.Email == usuario.Email);
            if (existe) 
            {
                return (false, "El correo eletronico ingresado ya se encuentra registrado");
            
            }

            var n_usuario = new Usuarios
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                IdRol = 2,
                FechaRegistro = DateTime.Now,
                Activo = true

            };

            n_usuario.ContrasenaHash = _passwordHasher.HashPassword(n_usuario, usuario.Contrasena);
            _context.Usuarios.Add(n_usuario);
            await _context.SaveChangesAsync();

            return (true, "El usuario ha sido registrado correctamente");
        
        
        }
    }
}
