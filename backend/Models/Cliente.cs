using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("nombre_cliente")]
        [StringLength(80)]
        public string NombreCliente { get; set; } = string.Empty;

        [Column("dni")]
        [StringLength(20)]
        public string? Dni { get; set; }

        [Column("direccion")]
        [StringLength(150)]
        public string? Direccion { get; set; }

        [Required]
        [Column("telefono")]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Column("email")]
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Column("notas")]
        public string? Notas { get; set; }

        // Propiedades de navegación
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}