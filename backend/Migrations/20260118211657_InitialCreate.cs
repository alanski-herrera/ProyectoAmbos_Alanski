using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProyectoAmbos_Alanski.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    id_admin = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_admin = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dni = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contrasena = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ultimo_acceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.id_admin);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_cliente = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    dni = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    direccion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    notas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.id_cliente);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                columns: table => new
                {
                    id_marca = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.id_marca);
                });

            migrationBuilder.CreateTable(
                name: "Tipos_Prenda",
                columns: table => new
                {
                    id_tipo_prenda = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipos_Prenda", x => x.id_tipo_prenda);
                });

            migrationBuilder.CreateTable(
                name: "Uniformes",
                columns: table => new
                {
                    id_uniforme = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    talle = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    precio = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    id_marca = table.Column<int>(type: "integer", nullable: false),
                    id_tipo_prenda = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    imagen_1 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    imagen_2 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    imagen_3 = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaReserva = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uniformes", x => x.id_uniforme);
                    table.ForeignKey(
                        name: "FK_Uniformes_Marcas_id_marca",
                        column: x => x.id_marca,
                        principalTable: "Marcas",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uniformes_Tipos_Prenda_id_tipo_prenda",
                        column: x => x.id_tipo_prenda,
                        principalTable: "Tipos_Prenda",
                        principalColumn: "id_tipo_prenda",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    id_reserva = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_uniforme = table.Column<int>(type: "integer", nullable: false),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    fecha_reserva = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado_reserva = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    mensaje_whatsapp = table.Column<string>(type: "text", nullable: true),
                    fecha_vencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notas = table.Column<string>(type: "text", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.id_reserva);
                    table.ForeignKey(
                        name: "FK_Reservas_Clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Uniformes_id_uniforme",
                        column: x => x.id_uniforme,
                        principalTable: "Uniformes",
                        principalColumn: "id_uniforme",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    id_venta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_uniforme = table.Column<int>(type: "integer", nullable: false),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    id_reserva = table.Column<int>(type: "integer", nullable: true),
                    fecha_venta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    monto_total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    metodo_pago = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    comprobante_pago = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    confirmado = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_confirmacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notas = table.Column<string>(type: "text", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.id_venta);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_Reservas_id_reserva",
                        column: x => x.id_reserva,
                        principalTable: "Reservas",
                        principalColumn: "id_reserva",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Ventas_Uniformes_id_uniforme",
                        column: x => x.id_uniforme,
                        principalTable: "Uniformes",
                        principalColumn: "id_uniforme",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Envios",
                columns: table => new
                {
                    id_envio = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_venta = table.Column<int>(type: "integer", nullable: false),
                    direccion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ciudad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    provincia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    codigo_postal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    fecha_envio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_entrega_estimada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estado_envio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    empresa_envio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    numero_seguimiento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    costo_envio = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    notas = table.Column<string>(type: "text", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_modificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envios", x => x.id_envio);
                    table.ForeignKey(
                        name: "FK_Envios_Ventas_id_venta",
                        column: x => x.id_venta,
                        principalTable: "Ventas",
                        principalColumn: "id_venta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_dni",
                table: "Administradores",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_email",
                table: "Administradores",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_email",
                table: "Clientes",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_telefono",
                table: "Clientes",
                column: "telefono");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_estado_envio",
                table: "Envios",
                column: "estado_envio");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_fecha_envio",
                table: "Envios",
                column: "fecha_envio");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_id_venta",
                table: "Envios",
                column: "id_venta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marcas_nombre_marca",
                table: "Marcas",
                column: "nombre_marca",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_estado_reserva",
                table: "Reservas",
                column: "estado_reserva");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_fecha_reserva",
                table: "Reservas",
                column: "fecha_reserva");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_id_cliente",
                table: "Reservas",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_id_uniforme",
                table: "Reservas",
                column: "id_uniforme");

            migrationBuilder.CreateIndex(
                name: "IX_Tipos_Prenda_nombre_tipo",
                table: "Tipos_Prenda",
                column: "nombre_tipo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uniformes_estado",
                table: "Uniformes",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_Uniformes_fecha_ingreso",
                table: "Uniformes",
                column: "fecha_ingreso");

            migrationBuilder.CreateIndex(
                name: "IX_Uniformes_id_marca",
                table: "Uniformes",
                column: "id_marca");

            migrationBuilder.CreateIndex(
                name: "IX_Uniformes_id_tipo_prenda",
                table: "Uniformes",
                column: "id_tipo_prenda");

            migrationBuilder.CreateIndex(
                name: "IX_Uniformes_precio",
                table: "Uniformes",
                column: "precio");

            migrationBuilder.CreateIndex(
                name: "IX_Uniformes_talle",
                table: "Uniformes",
                column: "talle");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_confirmado",
                table: "Ventas",
                column: "confirmado");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_fecha_venta",
                table: "Ventas",
                column: "fecha_venta");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_id_cliente",
                table: "Ventas",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_id_reserva",
                table: "Ventas",
                column: "id_reserva");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_id_uniforme",
                table: "Ventas",
                column: "id_uniforme");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Envios");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Uniformes");

            migrationBuilder.DropTable(
                name: "Marcas");

            migrationBuilder.DropTable(
                name: "Tipos_Prenda");
        }
    }
}
