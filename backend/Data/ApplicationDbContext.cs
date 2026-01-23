using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Models;

namespace ProyectoAmbos_Alanski.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Representan las tablas de la base de datos
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<TipoPrenda> TiposPrenda { get; set; }
        public DbSet<Uniforme> Uniformes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Envio> Envios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Configuración de Administrador =====
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.IdAdmin);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Dni).IsUnique();
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // ===== Configuración de Cliente =====
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);
                entity.HasIndex(e => e.Telefono);
                entity.HasIndex(e => e.Email);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // ===== Configuración de Marca =====
            modelBuilder.Entity<Marca>(entity =>
            {
                entity.HasKey(e => e.IdMarca);
                entity.HasIndex(e => e.NombreMarca).IsUnique();
            });

            // ===== Configuración de TipoPrenda =====
            modelBuilder.Entity<TipoPrenda>(entity =>
            {
                entity.HasKey(e => e.IdTipoPrenda);
                entity.HasIndex(e => e.NombreTipo).IsUnique();
            });

            // ===== Configuración de Uniforme =====
            modelBuilder.Entity<Uniforme>(entity =>
            {
                entity.HasKey(e => e.IdUniforme);

                entity.HasIndex(e => e.Estado);
                entity.HasIndex(e => e.Talle);
                entity.HasIndex(e => e.Precio);
                entity.HasIndex(e => e.FechaIngreso);

                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FechaIngreso)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                // Relación con Marca
                entity.HasOne(u => u.Marca)
                    .WithMany(m => m.Uniformes)
                    .HasForeignKey(u => u.IdMarca)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con TipoPrenda
                entity.HasOne(u => u.TipoPrenda)
                    .WithMany(t => t.Uniformes)
                    .HasForeignKey(u => u.IdTipoPrenda)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Configuración de Reserva =====
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(e => e.IdReserva);

                entity.HasIndex(e => e.EstadoReserva);
                entity.HasIndex(e => e.FechaReserva);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                // Relación con Uniforme
                entity.HasOne(r => r.Uniforme)
                    .WithMany(u => u.Reservas)
                    .HasForeignKey(r => r.IdUniforme)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Cliente
                entity.HasOne(r => r.Cliente)
                    .WithMany(c => c.Reservas)
                    .HasForeignKey(r => r.IdCliente)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== Configuración de Venta =====
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.IdVenta);

                entity.HasIndex(e => e.FechaVenta);
                entity.HasIndex(e => e.Confirmado);

                entity.Property(e => e.MontoTotal)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Uniforme
                entity.HasOne(v => v.Uniforme)
                    .WithMany(u => u.Ventas)
                    .HasForeignKey(v => v.IdUniforme)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Cliente
                entity.HasOne(v => v.Cliente)
                    .WithMany(c => c.Ventas)
                    .HasForeignKey(v => v.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Reserva (opcional)
                entity.HasOne(v => v.Reserva)
                    .WithMany(r => r.Ventas)
                    .HasForeignKey(v => v.IdReserva)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ===== Configuración de Envio =====
            modelBuilder.Entity<Envio>(entity =>
            {
                entity.HasKey(e => e.IdEnvio);

                entity.HasIndex(e => e.EstadoEnvio);
                entity.HasIndex(e => e.FechaEnvio);

                entity.Property(e => e.CostoEnvio)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                // Relación con Venta (uno a uno)
                entity.HasOne(e => e.Venta)
                    .WithOne(v => v.Envio)
                    .HasForeignKey<Envio>(e => e.IdVenta)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        }

    }
}