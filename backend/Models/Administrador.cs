using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAmbos_Alanski.Models
{
    [Table("Administradores")]
    public class Administrador
    {
        [Key]
        [Column("id_admin")]
        public int IdAdmin { get; set; }

        [Required]
        [Column("nombre_admin")]
        [StringLength(50)]
        public string NombreAdmin { get; set; } = string.Empty;

        [Required]
        [Column("dni")]
        [StringLength(20)]
        public string Dni { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("contrasena")]
        [StringLength(255)]
        public string Contrasena { get; set; } = string.Empty;

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }  // ← Agregar ?

        [Column("ultimo_acceso")]
        public DateTime? UltimoAcceso { get; set; }  // ← Agregar ?

        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}