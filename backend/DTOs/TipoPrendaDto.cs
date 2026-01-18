using System.ComponentModel.DataAnnotations;

namespace ProyectoAmbos_Alanski.DTOs
{
    public class TipoPrendaCreateDto
    {
        [Required(ErrorMessage = "El nombre del tipo es obligatorio")]
        [StringLength(50)]
        public string NombreTipo { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }
    }

    public class TipoPrendaUpdateDto
    {
        [Required]
        public int IdTipoPrenda { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreTipo { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}