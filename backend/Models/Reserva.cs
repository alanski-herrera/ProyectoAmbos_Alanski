using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        [Column("id_reserva")]
        public int IdReserva { get; set; }

        [Required]
        [Column("id_uniforme")]
        public int IdUniforme { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("fecha_reserva")]
        public DateTime FechaReserva { get; set; } = DateTime.Now;

        [Required]
        [Column("estado_reserva")]
        [StringLength(20)]
        public string EstadoReserva { get; set; } = "Activa"; // Activa, Cancelada, Confirmada

        [Column("mensaje_whatsapp")]
        public string? MensajeWhatsapp { get; set; }

        [Column("fecha_vencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("fecha_modificacion")]
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("IdUniforme")]
        public Uniforme Uniforme { get; set; } = null!;

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}