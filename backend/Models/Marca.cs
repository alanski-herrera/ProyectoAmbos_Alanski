using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Marcas")]
    public class Marca
    {
        [Key]
        [Column("id_marca")]
        public int IdMarca { get; set; }

        [Required]
        [Column("nombre_marca")]
        [StringLength(50)]
        public string NombreMarca { get; set; } = string.Empty;

        [Column("descripcion")]
        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        public ICollection<Uniforme> Uniformes { get; set; } = new List<Uniforme>();
    }
}