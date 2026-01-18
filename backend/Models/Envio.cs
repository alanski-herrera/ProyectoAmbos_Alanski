using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Envios")]
    public class Envio
    {
        [Key]
        [Column("id_envio")]
        public int IdEnvio { get; set; }

        [Required]
        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Required]
        [Column("direccion")]
        [StringLength(150)]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [Column("ciudad")]
        [StringLength(50)]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        [Column("provincia")]
        [StringLength(50)]
        public string Provincia { get; set; } = string.Empty;

        [Column("codigo_postal")]
        [StringLength(10)]
        public string? CodigoPostal { get; set; }

        [Column("fecha_envio")]
        public DateTime? FechaEnvio { get; set; }

        [Column("fecha_entrega_estimada")]
        public DateTime? FechaEntregaEstimada { get; set; }

        [Column("estado_envio")]
        [StringLength(30)]
        public string EstadoEnvio { get; set; } = "Pendiente"; // Pendiente, En Camino, Entregado, Cancelado

        [Column("empresa_envio")]
        [StringLength(100)]
        public string? EmpresaEnvio { get; set; }

        [Column("numero_seguimiento")]
        [StringLength(100)]
        public string? NumeroSeguimiento { get; set; }

        [Column("costo_envio", TypeName = "decimal(10,2)")]
        public decimal? CostoEnvio { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("fecha_modificacion")]
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("IdVenta")]
        public Venta Venta { get; set; } = null!;
    }
}