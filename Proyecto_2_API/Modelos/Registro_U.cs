using System.ComponentModel.DataAnnotations;
namespace Proyecto_2_API.Modelos
{
    public class Registro_U
    {
        [Required (ErrorMessage = "Ingrese su nombre")]
        [StringLength (100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese su correo correctamente")]
        [EmailAddress(ErrorMessage = "Por favor, ingrese un correo válido")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar una contraseña")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener un minimo de 8 caracteres")]
        public string Contrasena { get; set; } = string.Empty;

    }
}
