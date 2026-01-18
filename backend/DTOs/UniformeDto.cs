using System.ComponentModel.DataAnnotations;

namespace ProyectoAmbos_Alanski.DTOs
{
    public class UniformeCreateDto
    {
        [Required(ErrorMessage = "El talle es obligatorio")]
        [StringLength(10)]
        public string Talle { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "El tipo de prenda es obligatorio")]
        public int IdTipoPrenda { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [StringLength(500)]
        public string? Imagen1 { get; set; }

        [StringLength(500)]
        public string? Imagen2 { get; set; }

        [StringLength(500)]
        public string? Imagen3 { get; set; }
    }

    public class UniformeUpdateDto
    {
        [Required]
        public int IdUniforme { get; set; }

        [Required]
        [StringLength(10)]
        public string Talle { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Precio { get; set; }

        [Required]
        public int IdMarca { get; set; }

        [Required]
        public int IdTipoPrenda { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [StringLength(500)]
        public string? Imagen1 { get; set; }

        [StringLength(500)]
        public string? Imagen2 { get; set; }

        [StringLength(500)]
        public string? Imagen3 { get; set; }
    }

    public class UniformeEstadoDto
    {
        [Required]
        [RegularExpression("^(Disponible|Reservado|Vendido)$",
            ErrorMessage = "Estado debe ser: Disponible, Reservado o Vendido")]
        public string Estado { get; set; } = string.Empty;
    }

    public class UniformeResponseDto
    {
        public int IdUniforme { get; set; }
        public string Talle { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen1 { get; set; }
        public string? Imagen2 { get; set; }
        public string? Imagen3 { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string TipoPrenda { get; set; } = string.Empty;
    }
}