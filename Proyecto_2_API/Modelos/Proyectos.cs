using System.ComponentModel.DataAnnotations;

namespace Proyecto_2_API.Modelos
{
    public class Proyectos
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public DateTime FechaInicio { get; set; } 
        [Required]
        public DateTime? FechaFin { get;  set; }
        [Required]
        public int IdUsuarioCreador { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
        [Required]
        public bool Activo { get; set; }
        [Required]
        public int IdResponsable { get; set; }
        [Required]
        public int IdEstado { get;  set; }




    }
}
