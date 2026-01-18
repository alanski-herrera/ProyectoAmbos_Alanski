using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Tipos_Prenda")]
    public class TipoPrenda
    {
        [Key]
        [Column("id_tipo_prenda")]
        public int IdTipoPrenda { get; set; }

        [Required]
        [Column("nombre_tipo")]
        [StringLength(50)]
        public string NombreTipo { get; set; } = string.Empty;

        [Column("descripcion")]
        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        public ICollection<Uniforme> Uniformes { get; set; } = new List<Uniforme>();
    }
}