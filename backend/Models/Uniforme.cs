using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Uniformes")]
    public class Uniforme
    {
        [Key]
        [Column("id_uniforme")]
        public int IdUniforme { get; set; }

        [Required]
        [Column("talle")]
        [StringLength(10)]
        public string Talle { get; set; } = string.Empty;

        [Required]
        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        [Column("estado")]
        [StringLength(20)]
        public string Estado { get; set; } = "Disponible"; // Disponible, Reservado, Vendido

        [Required]
        [Column("fecha_ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        [Required]
        [Column("id_marca")]
        public int IdMarca { get; set; }

        [Required]
        [Column("id_tipo_prenda")]
        public int IdTipoPrenda { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("imagen_1")]
        [StringLength(500)]
        public string? Imagen1 { get; set; }

        [Column("imagen_2")]
        [StringLength(500)]
        public string? Imagen2 { get; set; }

        [Column("imagen_3")]
        [StringLength(500)]
        public string? Imagen3 { get; set; }

        [Column("fecha_modificacion")]
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        [ForeignKey("IdMarca")]
        public Marca? Marca { get; set; } = null!;

        [ForeignKey("IdTipoPrenda")]
        public TipoPrenda? TipoPrenda { get; set; } = null!;

        public DateTime? FechaReserva { get; set; }

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}