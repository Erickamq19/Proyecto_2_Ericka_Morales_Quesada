using System.ComponentModel.DataAnnotations;

namespace Proyecto_2_API.Modelos
{
    public class Tareas
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;  
        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public int IdProyecto { get; set; }
        [Required]
        public int IdUsuarioCreador { get; set; }
        [Required]
        public int? IdUsuarioAsignado { get; set; }
        [Required]
        public int IdEstado { get; set; }
        [Required]
        public int IdPrioridad { get; set; }
        [Required]
        public DateTime FechaCreacion { get;  set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime FechaLimite { get; set; }
        public DateTime? FechaCompletada { get; set; }
    }
}
