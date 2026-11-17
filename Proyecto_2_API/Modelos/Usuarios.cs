using System.ComponentModel.DataAnnotations;

namespace Proyecto_2_API.Modelos
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } 
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string ContrasenaHash { get; set; }
        [Required]
        public int IdRol { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        public bool Activo {  get; set; }
    }
}
