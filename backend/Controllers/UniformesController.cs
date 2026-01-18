using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Models;
using ProyectoAmbos_Alanski.DTOs;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment
using Microsoft.AspNetCore.Http;    // Necesario para IFormFile
using System.IO;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniformesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment; // Inyección del entorno

        public UniformesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ENDPOINT PARA SUBIR IMÁGENES (AGREGADO)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No se ha seleccionado ninguna imagen" });

            try
            {
                // Definir carpeta de destino (wwwroot/uploads)
                string webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadsFolder = Path.Combine(webRootPath, "uploads");

                // Crear carpeta si no existe
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generar nombre único para evitar colisiones
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Construir URL pública
                var request = HttpContext.Request;
                var imageUrl = $"/uploads/{uniqueFileName}";

                return Ok(new { url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno al subir la imagen: {ex.Message}" });
            }
        }

        // GET: api/Uniformes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UniformeResponseDto>>> GetUniformes(
            [FromQuery] string? talle,
            [FromQuery] int? idMarca,
            [FromQuery] int? idTipoPrenda,
            [FromQuery] decimal? precioMin,
            [FromQuery] decimal? precioMax,
            [FromQuery] string? estado)
        {
            var query = _context.Uniformes
                .Include(u => u.Marca)
                .Include(u => u.TipoPrenda)
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(talle))
                query = query.Where(u => u.Talle == talle);

            if (idMarca.HasValue)
                query = query.Where(u => u.IdMarca == idMarca.Value);

            if (idTipoPrenda.HasValue)
                query = query.Where(u => u.IdTipoPrenda == idTipoPrenda.Value);

            if (precioMin.HasValue)
                query = query.Where(u => u.Precio >= precioMin.Value);

            if (precioMax.HasValue)
                query = query.Where(u => u.Precio <= precioMax.Value);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(u => u.Estado == estado);
            else
                query = query.Where(u => u.Estado == "Disponible");

            var uniformes = await query
                .OrderByDescending(u => u.FechaIngreso)
                .Select(u => new UniformeResponseDto
                {
                    IdUniforme = u.IdUniforme,
                    Talle = u.Talle,
                    Precio = u.Precio,
                    Estado = u.Estado,
                    FechaIngreso = u.FechaIngreso,
                    Descripcion = u.Descripcion,
                    Imagen1 = u.Imagen1,
                    Imagen2 = u.Imagen2,
                    Imagen3 = u.Imagen3,
                    Marca = u.Marca.NombreMarca,
                    TipoPrenda = u.TipoPrenda.NombreTipo
                })
                .ToListAsync();

            return Ok(uniformes);
        }

        // GET: api/Uniformes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UniformeResponseDto>> GetUniforme(int id)
        {
            var uniforme = await _context.Uniformes
                .Include(u => u.Marca)
                .Include(u => u.TipoPrenda)
                .Where(u => u.IdUniforme == id)
                .Select(u => new UniformeResponseDto
                {
                    IdUniforme = u.IdUniforme,
                    Talle = u.Talle,
                    Precio = u.Precio,
                    Estado = u.Estado,
                    FechaIngreso = u.FechaIngreso,
                    Descripcion = u.Descripcion,
                    Imagen1 = u.Imagen1,
                    Imagen2 = u.Imagen2,
                    Imagen3 = u.Imagen3,
                    Marca = u.Marca.NombreMarca,
                    TipoPrenda = u.TipoPrenda.NombreTipo
                })
                .FirstOrDefaultAsync();

            if (uniforme == null)
            {
                return NotFound(new { message = "Uniforme no encontrado" });
            }

            return Ok(uniforme);
        }


        // POST: api/Uniformes
        [HttpPost]
        public async Task<ActionResult<UniformeResponseDto>> PostUniforme(UniformeCreateDto dto)
        {
            var uniforme = new Uniforme
            {
                Talle = dto.Talle,
                Precio = dto.Precio,
                IdMarca = dto.IdMarca,
                IdTipoPrenda = dto.IdTipoPrenda,
                Descripcion = dto.Descripcion,
                Imagen1 = dto.Imagen1,
                Imagen2 = dto.Imagen2,
                Imagen3 = dto.Imagen3,
                FechaIngreso = DateTime.Now,
                Estado = "Disponible"
            };

            _context.Uniformes.Add(uniforme);
            await _context.SaveChangesAsync();

            var response = await _context.Uniformes
                .Include(u => u.Marca)
                .Include(u => u.TipoPrenda)
                .Where(u => u.IdUniforme == uniforme.IdUniforme)
                .Select(u => new UniformeResponseDto
                {
                    IdUniforme = u.IdUniforme,
                    Talle = u.Talle,
                    Precio = u.Precio,
                    Estado = u.Estado,
                    FechaIngreso = u.FechaIngreso,
                    Descripcion = u.Descripcion,
                    Imagen1 = u.Imagen1,
                    Imagen2 = u.Imagen2,
                    Imagen3 = u.Imagen3,
                    Marca = u.Marca.NombreMarca,
                    TipoPrenda = u.TipoPrenda.NombreTipo
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetUniforme), new { id = uniforme.IdUniforme }, response);
        }

        // PUT: api/Uniformes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUniforme(int id, UniformeUpdateDto dto)
        {
            if (id != dto.IdUniforme)
            {
                return BadRequest(new { message = "ID no coincide" });
            }

            var uniforme = await _context.Uniformes.FindAsync(id);
            if (uniforme == null)
            {
                return NotFound(new { message = "Uniforme no encontrado" });
            }

            uniforme.Talle = dto.Talle;
            uniforme.Precio = dto.Precio;
            uniforme.IdMarca = dto.IdMarca;
            uniforme.IdTipoPrenda = dto.IdTipoPrenda;
            uniforme.Descripcion = dto.Descripcion;
            uniforme.Imagen1 = dto.Imagen1;
            uniforme.Imagen2 = dto.Imagen2;
            uniforme.Imagen3 = dto.Imagen3;
            uniforme.FechaModificacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UniformeExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // PATCH: api/Uniformes/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] UniformeEstadoDto dto)
        {
            var uniforme = await _context.Uniformes.FindAsync(id);
            if (uniforme == null)
            {
                return NotFound();
            }

            uniforme.Estado = dto.Estado;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Uniformes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUniforme(int id)
        {
            var uniforme = await _context.Uniformes.FindAsync(id);
            if (uniforme == null)
            {
                return NotFound();
            }

            _context.Uniformes.Remove(uniforme);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Uniformes/talles
        [HttpGet("talles")]
        public async Task<ActionResult<IEnumerable<string>>> GetTallesDisponibles()
        {
            var talles = await _context.Uniformes
                .Where(u => u.Estado == "Disponible")
                .Select(u => u.Talle)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return Ok(talles);
        }

        private bool UniformeExists(int id)
        {
            return _context.Uniformes.Any(e => e.IdUniforme == id);
        }
    }
}