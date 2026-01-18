using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Models;
using ProyectoAmbos_Alanski.DTOs;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetVentas(
            [FromQuery] bool? confirmado,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta)
        {
            var query = _context.Ventas
                .Include(v => v.Uniforme)
                    .ThenInclude(u => u.Marca)
                .Include(v => v.Uniforme)
                    .ThenInclude(u => u.TipoPrenda)
                .Include(v => v.Cliente)
                .Include(v => v.Envio)
                .AsQueryable();

            if (confirmado.HasValue)
                query = query.Where(v => v.Confirmado == confirmado.Value);

            if (fechaDesde.HasValue)
                query = query.Where(v => v.FechaVenta >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(v => v.FechaVenta <= fechaHasta.Value);

            var ventas = await query
                .OrderByDescending(v => v.FechaVenta)
                .Select(v => new VentaResponseDto
                {
                    IdVenta = v.IdVenta,
                    FechaVenta = v.FechaVenta,
                    MontoTotal = v.MontoTotal,
                    MetodoPago = v.MetodoPago,
                    Confirmado = v.Confirmado,
                    FechaConfirmacion = v.FechaConfirmacion,
                    ComprobantePago = v.ComprobantePago,
                    Cliente = new ClienteSimpleDto
                    {
                        IdCliente = v.Cliente.IdCliente,
                        NombreCliente = v.Cliente.NombreCliente,
                        Telefono = v.Cliente.Telefono,
                        Email = v.Cliente.Email
                    },
                    Uniforme = new UniformeSimpleDto
                    {
                        IdUniforme = v.Uniforme.IdUniforme,
                        Talle = v.Uniforme.Talle,
                        Precio = v.Uniforme.Precio,
                        Marca = v.Uniforme.Marca.NombreMarca,
                        TipoPrenda = v.Uniforme.TipoPrenda.NombreTipo,
                        Imagen1 = v.Uniforme.Imagen1
                    },
                    EstadoEnvio = v.Envio != null ? v.Envio.EstadoEnvio : "Sin Envío"
                })
                .ToListAsync();

            return Ok(ventas);
        }

        // GET: api/Ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Uniforme)
                    .ThenInclude(u => u.Marca)
                .Include(v => v.Uniforme)
                    .ThenInclude(u => u.TipoPrenda)
                .Include(v => v.Cliente)
                .Include(v => v.Reserva)
                .Include(v => v.Envio)
                .Where(v => v.IdVenta == id)
                .FirstOrDefaultAsync();

            if (venta == null)
            {
                return NotFound(new { message = "Venta no encontrada" });
            }

            return Ok(venta);
        }

        // En tu VentaController o UniformeController
        [HttpPost("reservar/{id}")]
        public async Task<IActionResult> Reservar(int id)
        {
            var uniforme = await _context.Uniformes.FindAsync(id);
            if (uniforme == null) return NotFound();
            if (uniforme.Estado != "Disponible")
                return BadRequest("El producto ya no está disponible.");
            uniforme.Estado = "Reservado";
            uniforme.FechaReserva = DateTime.Now; // Guardamos la hora actual
            await _context.SaveChangesAsync();
            return Ok();
        }

        // POST: api/Ventas
        [HttpPost]
        public async Task<ActionResult<Venta>> PostVenta(VentaCreateDto dto)
        {
            var uniforme = await _context.Uniformes.FindAsync(dto.IdUniforme);
            if (uniforme == null)
            {
                return NotFound(new { message = "Uniforme no encontrado" });
            }

            var venta = new Venta
            {
                IdUniforme = dto.IdUniforme,
                IdCliente = dto.IdCliente,
                IdReserva = dto.IdReserva,
                FechaVenta = DateTime.Now,
                MontoTotal = dto.MontoTotal,
                MetodoPago = dto.MetodoPago,
                ComprobantePago = dto.ComprobantePago,
                Notas = dto.Notas,
                Confirmado = false,
                FechaCreacion = DateTime.Now
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVenta), new { id = venta.IdVenta }, venta);
        }

        // POST: api/Ventas/rapida
        [HttpPost("rapida")]
        public async Task<ActionResult<Venta>> PostVentaRapida(VentaRapidaDto dto)
        {
            var uniforme = await _context.Uniformes.FindAsync(dto.IdUniforme);
            if (uniforme == null)
            {
                return NotFound(new { message = "Uniforme no encontrado" });
            }
            // --- LÓGICA DE VALIDACIÓN DE RESERVA ---
            if (uniforme.Estado != "Disponible")
            {
                return BadRequest(new { message = "Este producto ya no está disponible (ha sido reservado o vendido)." });
            }
            // ---------------------------------------
            // Buscar o crear el cliente
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Telefono == dto.Telefono);
            if (cliente == null)
            {
                cliente = new Cliente
                {
                    NombreCliente = dto.NombreCliente,
                    Telefono = dto.Telefono,
                    Email = dto.Email,
                    FechaRegistro = DateTime.Now
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            var venta = new Venta
            {
                IdUniforme = dto.IdUniforme,
                IdCliente = cliente.IdCliente,
                IdReserva = dto.IdReserva,
                FechaVenta = DateTime.Now,
                MontoTotal = dto.MontoTotal,
                MetodoPago = dto.MetodoPago,
                ComprobantePago = dto.ComprobantePago,
                Notas = dto.Notas,
                Confirmado = false,
                FechaCreacion = DateTime.Now
            };
            // --- ACTUALIZAR ESTADO DEL UNIFORME ---
            uniforme.Estado = "Reservado";
            uniforme.FechaReserva = DateTime.Now; // Asegúrate de tener esta columna en BD
            // --------------------------------------
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVenta), new { id = venta.IdVenta }, venta);
        }

        // PATCH: api/Ventas/5/confirmar
        [HttpPatch("{id}/confirmar")]
        public async Task<IActionResult> ConfirmarPago(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Uniforme)
                .Include(v => v.Reserva)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            venta.Confirmado = true;
            venta.FechaConfirmacion = DateTime.Now;
            venta.Uniforme.Estado = "Vendido";

            if (venta.Reserva != null)
            {
                venta.Reserva.EstadoReserva = "Confirmada";
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Ventas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenta(int id, Venta venta)
        {
            if (id != venta.IdVenta)
            {
                return BadRequest(new { message = "ID no coincide" });
            }

            _context.Entry(venta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentaExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Ventas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Ventas/estadisticas
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticas()
        {
            var totalVentas = await _context.Ventas.CountAsync();
            var ventasConfirmadas = await _context.Ventas.CountAsync(v => v.Confirmado);
            var ventasPendientes = await _context.Ventas.CountAsync(v => !v.Confirmado);
            var montoTotal = await _context.Ventas
                .Where(v => v.Confirmado)
                .SumAsync(v => v.MontoTotal);

            return Ok(new
            {
                totalVentas,
                ventasConfirmadas,
                ventasPendientes,
                montoTotal
            });
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.IdVenta == id);
        }
    }
}