using System.ComponentModel.DataAnnotations;

namespace Proyecto_2_API.Modelos
{
    public class Inicio_U
    {
        [Required (ErrorMessage = "Ingrese su correo electrónico")]
        [EmailAddress (ErrorMessage = "Ingrese un correo válido")]
        public string Email { get; set; } = string.Empty;

        [Required (ErrorMessage = "Ingrese su contraseña")]
        public string Contrasena {  get; set; } = string.Empty; 

    }
}
