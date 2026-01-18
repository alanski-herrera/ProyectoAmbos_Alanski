using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Models;
using ProyectoAmbos_Alanski.DTOs;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposPrendaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TiposPrendaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TiposPrenda
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoPrenda>>> GetTiposPrenda()
        {
            return await _context.TiposPrenda
                .Where(t => t.Activo)
                .OrderBy(t => t.NombreTipo)
                .ToListAsync();
        }

        // GET: api/TiposPrenda/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoPrenda>> GetTipoPrenda(int id)
        {
            var tipoPrenda = await _context.TiposPrenda.FindAsync(id);

            if (tipoPrenda == null)
            {
                return NotFound(new { message = "Tipo de prenda no encontrado" });
            }

            return tipoPrenda;
        }

        // POST: api/TiposPrenda
        [HttpPost]
        public async Task<ActionResult<TipoPrenda>> PostTipoPrenda(TipoPrendaCreateDto dto)
        {
            var tipoPrenda = new TipoPrenda
            {
                NombreTipo = dto.NombreTipo,
                Descripcion = dto.Descripcion,
                Activo = true
            };

            _context.TiposPrenda.Add(tipoPrenda);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTipoPrenda), new { id = tipoPrenda.IdTipoPrenda }, tipoPrenda);
        }

        // PUT: api/TiposPrenda/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoPrenda(int id, TipoPrendaUpdateDto dto)
        {
            if (id != dto.IdTipoPrenda)
            {
                return BadRequest(new { message = "ID no coincide" });
            }

            var tipoPrenda = await _context.TiposPrenda.FindAsync(id);
            if (tipoPrenda == null)
            {
                return NotFound(new { message = "Tipo de prenda no encontrado" });
            }

            tipoPrenda.NombreTipo = dto.NombreTipo;
            tipoPrenda.Descripcion = dto.Descripcion;
            tipoPrenda.Activo = dto.Activo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoPrendaExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/TiposPrenda/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoPrenda(int id)
        {
            var tipoPrenda = await _context.TiposPrenda.FindAsync(id);
            if (tipoPrenda == null)
            {
                return NotFound();
            }

            tipoPrenda.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoPrendaExists(int id)
        {
            return _context.TiposPrenda.Any(e => e.IdTipoPrenda == id);
        }
    }
}