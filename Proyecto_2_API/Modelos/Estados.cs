using System.ComponentModel.DataAnnotations;

namespace Proyecto_2_API.Modelos
{
    public class Estados
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
        [StringLength(150)]
        public string Descripcion { get; set; }

    }
}
