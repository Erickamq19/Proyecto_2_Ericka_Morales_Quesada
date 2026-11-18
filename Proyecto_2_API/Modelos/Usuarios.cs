using System.ComponentModel.DataAnnotations;

namespace Proyecto_2_API.Modelos
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string ContrasenaHash { get; set; } = string.Empty;
        [Required]
        public int IdRol { get; set; }
   
        public DateTime FechaRegistro { get; set; }
        
        public bool Activo {  get; set; }
    }
}
