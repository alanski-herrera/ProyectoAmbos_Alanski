using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Required]
        [Column("id_uniforme")]
        public int IdUniforme { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("id_reserva")]
        public int? IdReserva { get; set; }

        [Required]
        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Required]
        [Column("monto_total", TypeName = "decimal(10,2)")]
        public decimal MontoTotal { get; set; }

        [Column("metodo_pago")]
        [StringLength(20)]
        public string MetodoPago { get; set; } = "Transferencia"; // Transferencia, Efectivo, Otro

        [Column("comprobante_pago")]
        [StringLength(500)]
        public string? ComprobantePago { get; set; }

        [Column("confirmado")]
        public bool Confirmado { get; set; } = false;

        [Column("fecha_confirmacion")]
        public DateTime? FechaConfirmacion { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("IdUniforme")]
        public Uniforme Uniforme { get; set; } = null!;

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; } = null!;

        [ForeignKey("IdReserva")]
        public Reserva? Reserva { get; set; }

        public Envio? Envio { get; set; }
    }
}