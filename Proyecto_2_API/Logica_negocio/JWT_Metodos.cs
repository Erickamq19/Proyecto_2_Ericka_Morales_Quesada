using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proyecto_2_API.DB;
using Proyecto_2_API.Interfaces;
using Proyecto_2_API.Modelos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Proyecto_2_API.Logica_negocio
{
    public class JWT_Metodos : Interface_JWT
    {
        private readonly conexion_DB _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Usuarios> _passwordHasher;

        public JWT_Metodos (conexion_DB context, IConfiguration configuration, PasswordHasher<Usuarios> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<string?> Inicio_S_Async(Inicio_U usuario) 
        {
            string correo = usuario.Email.Trim();
            var n_usuario = await _context.Usuarios.FirstOrDefaultAsync(i => i.Email == correo);
            if (n_usuario == null || !n_usuario.Activo) 
            {
                return null;

            }

            var resultado = _passwordHasher.VerifyHashedPassword(n_usuario, n_usuario.ContrasenaHash, usuario.Contrasena);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return null;


            }

            var rol = await _context.Roles.FirstOrDefaultAsync(y => y.Id == n_usuario.IdRol);
            if (rol == null) 
            { 
                return null;
            }

            var claims = new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, n_usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, n_usuario.Nombre),
                        new Claim(ClaimTypes.Email, n_usuario.Email),
                        new Claim(ClaimTypes.Role, rol.Nombre)

                    };
            var clave = _configuration["Jwt:Key"];
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var minutos = Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]);
            var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"], audience: _configuration["Jwt:Audience"], claims: claims, expires: DateTime.Now.AddMinutes(minutos),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
