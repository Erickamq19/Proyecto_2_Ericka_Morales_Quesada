using System.ComponentModel.DataAnnotations;

namespace Proyecto_2_API.Modelos
{
    public class Prioridades
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Nombre { get; set; }
        [StringLength(150)]
        public string Descricao { get; set; }


    }
}
