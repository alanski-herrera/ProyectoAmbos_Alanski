using System.ComponentModel.DataAnnotations;

namespace ProyectoAmbos_Alanski.DTOs
{
    public class EnvioCreateDto
    {
        [Required(ErrorMessage = "La venta es obligatoria")]
        public int IdVenta { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(150)]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(50)]
        public string Ciudad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La provincia es obligatoria")]
        [StringLength(50)]
        public string Provincia { get; set; } = string.Empty;

        [StringLength(10)]
        public string? CodigoPostal { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public DateTime? FechaEntregaEstimada { get; set; }

        [StringLength(100)]
        public string? EmpresaEnvio { get; set; }

        [StringLength(100)]
        public string? NumeroSeguimiento { get; set; }

        [Range(0, 999999.99)]
        public decimal? CostoEnvio { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    public class EnvioUpdateDto
    {
        [Required]
        public int IdEnvio { get; set; }

        [Required]
        public int IdVenta { get; set; }

        [Required]
        [StringLength(150)]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Provincia { get; set; } = string.Empty;

        [StringLength(10)]
        public string? CodigoPostal { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public DateTime? FechaEntregaEstimada { get; set; }

        [StringLength(100)]
        public string? EmpresaEnvio { get; set; }

        [StringLength(100)]
        public string? NumeroSeguimiento { get; set; }

        [Range(0, 999999.99)]
        public decimal? CostoEnvio { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    public class EnvioEstadoDto
    {
        [Required]
        [RegularExpression("^(Pendiente|En Camino|Entregado|Cancelado)$",
            ErrorMessage = "Estado debe ser: Pendiente, En Camino, Entregado o Cancelado")]
        public string Estado { get; set; } = string.Empty;
    }

    public class EnvioResponseDto
    {
        public int IdEnvio { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string? CodigoPostal { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaEntregaEstimada { get; set; }
        public string EstadoEnvio { get; set; } = string.Empty;
        public string? EmpresaEnvio { get; set; }
        public string? NumeroSeguimiento { get; set; }
        public decimal? CostoEnvio { get; set; }
        public string? Notas { get; set; }
    }
}