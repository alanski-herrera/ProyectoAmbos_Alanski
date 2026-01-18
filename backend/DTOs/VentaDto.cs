using System.ComponentModel.DataAnnotations;

namespace ProyectoAmbos_Alanski.DTOs
{
    public class VentaCreateDto
    {
        [Required(ErrorMessage = "El uniforme es obligatorio")]
        public int IdUniforme { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int IdCliente { get; set; }

        public int? IdReserva { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, 999999.99)]
        public decimal MontoTotal { get; set; }

        [Required]
        [RegularExpression("^(Transferencia|Efectivo|Otro)$")]
        public string MetodoPago { get; set; } = "Transferencia";

        [StringLength(500)]
        public string? ComprobantePago { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    public class VentaRapidaDto
    {
        // Datos del uniforme
        [Required]
        public int IdUniforme { get; set; }

        // Datos del cliente (se crea si no existe)
        [Required]
        [StringLength(80)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public int? IdReserva { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal MontoTotal { get; set; }

        [Required]
        public string MetodoPago { get; set; } = "Transferencia";

        [StringLength(500)]
        public string? ComprobantePago { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    public class VentaResponseDto
    {
        public int IdVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal MontoTotal { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public bool Confirmado { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public string? ComprobantePago { get; set; }

        public ClienteSimpleDto Cliente { get; set; } = null!;
        public UniformeSimpleDto Uniforme { get; set; } = null!;
        public string EstadoEnvio { get; set; } = "Sin Envío";
    }
}