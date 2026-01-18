using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Models;
using ProyectoAmbos_Alanski.DTOs;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MarcasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Marcas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marca>>> GetMarcas()
        {
            return await _context.Marcas
                .Where(m => m.Activo)
                .OrderBy(m => m.NombreMarca)
                .ToListAsync();
        }

        // GET: api/Marcas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Marca>> GetMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
            {
                return NotFound(new { message = "Marca no encontrada" });
            }

            return marca;
        }

        // POST: api/Marcas
        [HttpPost]
        public async Task<ActionResult<Marca>> PostMarca(MarcaCreateDto dto)
        {
            var marca = new Marca
            {
                NombreMarca = dto.NombreMarca,
                Descripcion = dto.Descripcion,
                Activo = true
            };

            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMarca), new { id = marca.IdMarca }, marca);
        }

        // PUT: api/Marcas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMarca(int id, MarcaUpdateDto dto)
        {
            if (id != dto.IdMarca)
            {
                return BadRequest(new { message = "ID no coincide" });
            }

            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound(new { message = "Marca no encontrada" });
            }

            marca.NombreMarca = dto.NombreMarca;
            marca.Descripcion = dto.Descripcion;
            marca.Activo = dto.Activo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarcaExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Marcas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound();
            }

            marca.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MarcaExists(int id)
        {
            return _context.Marcas.Any(e => e.IdMarca == id);
        }
    }
}