using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Models;
using ProyectoAmbos_Alanski.DTOs;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservaResponseDto>>> GetReservas([FromQuery] string? estado)
        {
            var query = _context.Reservas
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.Marca)
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.TipoPrenda)
                .Include(r => r.Cliente)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(r => r.EstadoReserva == estado);

            var reservas = await query
                .OrderByDescending(r => r.FechaReserva)
                .Select(r => new ReservaResponseDto
                {
                    IdReserva = r.IdReserva,
                    FechaReserva = r.FechaReserva,
                    FechaVencimiento = r.FechaVencimiento,
                    EstadoReserva = r.EstadoReserva,
                    MensajeWhatsapp = r.MensajeWhatsapp,
                    Notas = r.Notas,
                    Cliente = new ClienteSimpleDto
                    {
                        IdCliente = r.Cliente.IdCliente,
                        NombreCliente = r.Cliente.NombreCliente,
                        Telefono = r.Cliente.Telefono,
                        Email = r.Cliente.Email,
                        Direccion = r.Cliente.Direccion
                    },
                    Uniforme = new UniformeSimpleDto
                    {
                        IdUniforme = r.Uniforme.IdUniforme,
                        Talle = r.Uniforme.Talle,
                        Precio = r.Uniforme.Precio,
                        Marca = r.Uniforme.Marca.NombreMarca,
                        TipoPrenda = r.Uniforme.TipoPrenda.NombreTipo,
                        Imagen1 = r.Uniforme.Imagen1
                    }
                })
                .ToListAsync();

            return Ok(reservas);
        }

        // GET: api/Reservas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservaResponseDto>> GetReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.Marca)
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.TipoPrenda)
                .Include(r => r.Cliente)
                .Where(r => r.IdReserva == id)
                .Select(r => new ReservaResponseDto
                {
                    IdReserva = r.IdReserva,
                    FechaReserva = r.FechaReserva,
                    FechaVencimiento = r.FechaVencimiento,
                    EstadoReserva = r.EstadoReserva,
                    MensajeWhatsapp = r.MensajeWhatsapp,
                    Notas = r.Notas,
                    Cliente = new ClienteSimpleDto
                    {
                        IdCliente = r.Cliente.IdCliente,
                        NombreCliente = r.Cliente.NombreCliente,
                        Telefono = r.Cliente.Telefono,
                        Email = r.Cliente.Email,
                        Direccion = r.Cliente.Direccion
                    },
                    Uniforme = new UniformeSimpleDto
                    {
                        IdUniforme = r.Uniforme.IdUniforme,
                        Talle = r.Uniforme.Talle,
                        Precio = r.Uniforme.Precio,
                        Marca = r.Uniforme.Marca.NombreMarca,
                        TipoPrenda = r.Uniforme.TipoPrenda.NombreTipo,
                        Imagen1 = r.Uniforme.Imagen1
                    }
                })
                .FirstOrDefaultAsync();

            if (reserva == null)
            {
                return NotFound(new { message = "Reserva no encontrada" });
            }

            return Ok(reserva);
        }

        // POST: api/Reservas
        [HttpPost]
        public async Task<ActionResult<ReservaResponseDto>> PostReserva(ReservaCreateDto dto)
        {
            var uniforme = await _context.Uniformes.FindAsync(dto.IdUniforme);
            if (uniforme == null)
            {
                return NotFound(new { message = "Uniforme no encontrado" });
            }

            if (uniforme.Estado != "Disponible")
            {
                return BadRequest(new { message = "El uniforme no está disponible" });
            }

            var reserva = new Reserva
            {
                IdUniforme = dto.IdUniforme,
                IdCliente = dto.IdCliente,
                FechaReserva = DateTime.Now,
                EstadoReserva = "Activa",
                MensajeWhatsapp = dto.MensajeWhatsapp,
                FechaVencimiento = dto.FechaVencimiento ?? DateTime.Now.AddDays(3),
                Notas = dto.Notas,
                FechaCreacion = DateTime.Now
            };

            uniforme.Estado = "Reservado";

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            var response = await _context.Reservas
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.Marca)
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.TipoPrenda)
                .Include(r => r.Cliente)
                .Where(r => r.IdReserva == reserva.IdReserva)
                .Select(r => new ReservaResponseDto
                {
                    IdReserva = r.IdReserva,
                    FechaReserva = r.FechaReserva,
                    FechaVencimiento = r.FechaVencimiento,
                    EstadoReserva = r.EstadoReserva,
                    MensajeWhatsapp = r.MensajeWhatsapp,
                    Notas = r.Notas,
                    Cliente = new ClienteSimpleDto
                    {
                        IdCliente = r.Cliente.IdCliente,
                        NombreCliente = r.Cliente.NombreCliente,
                        Telefono = r.Cliente.Telefono,
                        Email = r.Cliente.Email,
                        Direccion = r.Cliente.Direccion
                    },
                    Uniforme = new UniformeSimpleDto
                    {
                        IdUniforme = r.Uniforme.IdUniforme,
                        Talle = r.Uniforme.Talle,
                        Precio = r.Uniforme.Precio,
                        Marca = r.Uniforme.Marca.NombreMarca,
                        TipoPrenda = r.Uniforme.TipoPrenda.NombreTipo,
                        Imagen1 = r.Uniforme.Imagen1
                    }
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = reserva.IdReserva }, response);
        }

        // POST: api/Reservas/rapida
        [HttpPost("rapida")]
        public async Task<ActionResult<ReservaResponseDto>> PostReservaRapida(ReservaRapidaDto dto)
        {
            var uniforme = await _context.Uniformes.FindAsync(dto.IdUniforme);
            if (uniforme == null)
            {
                return NotFound(new { message = "Uniforme no encontrado" });
            }

            if (uniforme.Estado != "Disponible")
            {
                return BadRequest(new { message = "El uniforme no está disponible" });
            }

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
                    Direccion = dto.Direccion,
                    Dni = dto.Dni,
                    FechaRegistro = DateTime.Now
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Actualizar datos del cliente si ya existe
                cliente.NombreCliente = dto.NombreCliente;
                if (!string.IsNullOrEmpty(dto.Email)) cliente.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Email)) cliente.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Direccion)) cliente.Direccion = dto.Direccion;
                if (!string.IsNullOrEmpty(dto.Dni)) cliente.Dni = dto.Dni;
                await _context.SaveChangesAsync();
            }

            var reserva = new Reserva
            {
                IdUniforme = dto.IdUniforme,
                IdCliente = cliente.IdCliente,
                FechaReserva = DateTime.Now,
                EstadoReserva = "Activa",
                MensajeWhatsapp = dto.MensajeWhatsapp,
                FechaVencimiento = DateTime.Now.AddDays(3),
                Notas = dto.Notas,
                FechaCreacion = DateTime.Now
            };

            uniforme.Estado = "Reservado";

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            var response = await _context.Reservas
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.Marca)
                .Include(r => r.Uniforme)
                    .ThenInclude(u => u.TipoPrenda)
                .Include(r => r.Cliente)
                .Where(r => r.IdReserva == reserva.IdReserva)
                .Select(r => new ReservaResponseDto
                {
                    IdReserva = r.IdReserva,
                    FechaReserva = r.FechaReserva,
                    FechaVencimiento = r.FechaVencimiento,
                    EstadoReserva = r.EstadoReserva,
                    MensajeWhatsapp = r.MensajeWhatsapp,
                    Notas = r.Notas,
                    Cliente = new ClienteSimpleDto
                    {
                        IdCliente = r.Cliente.IdCliente,
                        NombreCliente = r.Cliente.NombreCliente,
                        Telefono = r.Cliente.Telefono,
                        Email = r.Cliente.Email,
                        Direccion = r.Cliente.Direccion
                    },
                    Uniforme = new UniformeSimpleDto
                    {
                        IdUniforme = r.Uniforme.IdUniforme,
                        Talle = r.Uniforme.Talle,
                        Precio = r.Uniforme.Precio,
                        Marca = r.Uniforme.Marca.NombreMarca,
                        TipoPrenda = r.Uniforme.TipoPrenda.NombreTipo,
                        Imagen1 = r.Uniforme.Imagen1
                    }
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = reserva.IdReserva }, response);
        }

        // PATCH: api/Reservas/5/confirmar
        [HttpPatch("{id}/confirmar")]
        public async Task<IActionResult> ConfirmarReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Uniforme)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            reserva.EstadoReserva = "Confirmada";
            reserva.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Reservas/5/cancelar
        [HttpPatch("{id}/cancelar")]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Uniforme)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            reserva.EstadoReserva = "Cancelada";
            reserva.FechaModificacion = DateTime.Now;
            reserva.Uniforme.Estado = "Disponible";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Reservas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Uniforme)
                .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null)
            {
                return NotFound();
            }

            if (reserva.EstadoReserva == "Activa")
            {
                reserva.Uniforme.Estado = "Disponible";
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Reservas/sincronizar
        [HttpPost("sincronizar")]
        public async Task<IActionResult> SincronizarReservas()
        {
            // Buscar uniformes en estado 'Reservado' que no tengan una reserva 'Activa'
            var uniformesHuerfanos = await _context.Uniformes
                .Where(u => u.Estado == "Reservado" && !_context.Reservas.Any(r => r.IdUniforme == u.IdUniforme && r.EstadoReserva == "Activa"))
                .ToListAsync();

            int creadas = 0;
            foreach (var uniforme in uniformesHuerfanos)
            {
                // Intentar buscar la última venta no confirmada para este uniforme para obtener el cliente
                var ultimaVenta = await _context.Ventas
                    .Include(v => v.Cliente)
                    .Where(v => v.IdUniforme == uniforme.IdUniforme && !v.Confirmado)
                    .OrderByDescending(v => v.FechaVenta)
                    .FirstOrDefaultAsync();

                if (ultimaVenta != null)
                {
                    var reserva = new Reserva
                    {
                        IdUniforme = uniforme.IdUniforme,
                        IdCliente = ultimaVenta.IdCliente,
                        FechaReserva = ultimaVenta.FechaVenta,
                        EstadoReserva = "Activa",
                        Notas = "Sincronizada automáticamente desde Venta existente",
                        FechaCreacion = DateTime.Now
                    };
                    _context.Reservas.Add(reserva);
                    creadas++;
                }
            }

            if (creadas > 0)
            {
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Se sincronizaron {creadas} reservas correctamente.", cantidad = creadas });
        }
    }
}