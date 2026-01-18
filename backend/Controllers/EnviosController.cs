using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Models;
using ProyectoAmbos_Alanski.DTOs;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnviosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnviosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Envios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnvioResponseDto>>> GetEnvios([FromQuery] string? estado)
        {
            var query = _context.Envios
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Cliente)
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Uniforme)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(e => e.EstadoEnvio == estado);

            var envios = await query
                .OrderByDescending(e => e.FechaCreacion)
                .Select(e => new EnvioResponseDto
                {
                    IdEnvio = e.IdEnvio,
                    Direccion = e.Direccion,
                    Ciudad = e.Ciudad,
                    Provincia = e.Provincia,
                    CodigoPostal = e.CodigoPostal,
                    FechaEnvio = e.FechaEnvio,
                    FechaEntregaEstimada = e.FechaEntregaEstimada,
                    EstadoEnvio = e.EstadoEnvio,
                    EmpresaEnvio = e.EmpresaEnvio,
                    NumeroSeguimiento = e.NumeroSeguimiento,
                    CostoEnvio = e.CostoEnvio,
                    Notas = e.Notas
                })
                .ToListAsync();

            return Ok(envios);
        }

        // GET: api/Envios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetEnvio(int id)
        {
            var envio = await _context.Envios
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Cliente)
                .Include(e => e.Venta)
                    .ThenInclude(v => v.Uniforme)
                        .ThenInclude(u => u.Marca)
                .Where(e => e.IdEnvio == id)
                .FirstOrDefaultAsync();

            if (envio == null)
            {
                return NotFound(new { message = "Envío no encontrado" });
            }

            return Ok(envio);
        }

        // POST: api/Envios
        [HttpPost]
        public async Task<ActionResult<Envio>> PostEnvio(EnvioCreateDto dto)
        {
            var venta = await _context.Ventas.FindAsync(dto.IdVenta);
            if (venta == null)
            {
                return NotFound(new { message = "Venta no encontrada" });
            }

            var envio = new Envio
            {
                IdVenta = dto.IdVenta,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Provincia = dto.Provincia,
                CodigoPostal = dto.CodigoPostal,
                FechaEnvio = dto.FechaEnvio,
                FechaEntregaEstimada = dto.FechaEntregaEstimada,
                EstadoEnvio = "Pendiente",
                EmpresaEnvio = dto.EmpresaEnvio,
                NumeroSeguimiento = dto.NumeroSeguimiento,
                CostoEnvio = dto.CostoEnvio,
                Notas = dto.Notas,
                FechaCreacion = DateTime.Now
            };

            _context.Envios.Add(envio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnvio), new { id = envio.IdEnvio }, envio);
        }

        // PUT: api/Envios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnvio(int id, EnvioUpdateDto dto)
        {
            if (id != dto.IdEnvio)
            {
                return BadRequest(new { message = "ID no coincide" });
            }

            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
            {
                return NotFound(new { message = "Envío no encontrado" });
            }

            envio.Direccion = dto.Direccion;
            envio.Ciudad = dto.Ciudad;
            envio.Provincia = dto.Provincia;
            envio.CodigoPostal = dto.CodigoPostal;
            envio.FechaEnvio = dto.FechaEnvio;
            envio.FechaEntregaEstimada = dto.FechaEntregaEstimada;
            envio.EmpresaEnvio = dto.EmpresaEnvio;
            envio.NumeroSeguimiento = dto.NumeroSeguimiento;
            envio.CostoEnvio = dto.CostoEnvio;
            envio.Notas = dto.Notas;
            envio.FechaModificacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnvioExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // PATCH: api/Envios/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstadoEnvio(int id, [FromBody] EnvioEstadoDto dto)
        {
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
            {
                return NotFound();
            }

            envio.EstadoEnvio = dto.Estado;
            envio.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Envios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvio(int id)
        {
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
            {
                return NotFound();
            }

            _context.Envios.Remove(envio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnvioExists(int id)
        {
            return _context.Envios.Any(e => e.IdEnvio == id);
        }
    }
}