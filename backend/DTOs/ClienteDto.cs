using System.ComponentModel.DataAnnotations;

namespace ProyectoAmbos_Alanski.DTOs
{
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(80)]
        public string NombreCliente { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Dni { get; set; }

        [StringLength(150)]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    public class ClienteUpdateDto
    {
        [Required]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(80)]
        public string NombreCliente { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Dni { get; set; }

        [StringLength(150)]
        public string? Direccion { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Notas { get; set; }
    }

    public class ClienteResponseDto
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string? Direccion { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Notas { get; set; }
        public int CantidadReservas { get; set; }
        public int CantidadCompras { get; set; }
    }
}