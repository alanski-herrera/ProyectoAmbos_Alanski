using System.ComponentModel.DataAnnotations;

namespace ProyectoAmbos_Alanski.DTOs
{
    public class ReservaCreateDto
    {
        [Required(ErrorMessage = "El uniforme es obligatorio")]
        public int IdUniforme { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int IdCliente { get; set; }

        public string? MensajeWhatsapp { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    // DTO para crear una reserva rápida con datos del cliente
    public class ReservaRapidaDto
    {
        // Datos del uniforme
        [Required(ErrorMessage = "El uniforme es obligatorio")]
        public int IdUniforme { get; set; }

        // Datos del cliente (se crea si no existe)
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(80)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Dni { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Direccion { get; set; }

        public string? MensajeWhatsapp { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    public class ReservaResponseDto
    {
        public int IdReserva { get; set; }
        public DateTime FechaReserva { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string EstadoReserva { get; set; } = string.Empty;
        public string? MensajeWhatsapp { get; set; }
        public string? Notas { get; set; }

        public ClienteSimpleDto Cliente { get; set; } = null!;
        public UniformeSimpleDto Uniforme { get; set; } = null!;
    }

    public class ClienteSimpleDto
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Direccion { get; set; }
    }

    public class UniformeSimpleDto
    {
        public int IdUniforme { get; set; }
        public string Talle { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string TipoPrenda { get; set; } = string.Empty;
        public string? Imagen1 { get; set; }
    }
}