using System.ComponentModel.DataAnnotations;

namespace ProyectoAmbos_Alanski.DTOs
{
    public class MarcaCreateDto
    {
        [Required(ErrorMessage = "El nombre de la marca es obligatorio")]
        [StringLength(50)]
        public string NombreMarca { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }
    }

    public class MarcaUpdateDto
    {
        [Required]
        public int IdMarca { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreMarca { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;
    }
}