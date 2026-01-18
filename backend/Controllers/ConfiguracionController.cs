using Microsoft.AspNetCore.Mvc;
using ProyectoAmbos_Alanski.Data;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const string COSTO_ENVIO_KEY = "CostoEnvioGlobal";

        private const string CONFIG_FILE = "configuracion.json";

        public ConfiguracionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Configuracion/envio
        [HttpGet("envio")]
        public ActionResult<ConfiguracionResponseDto> GetCostoEnvio()
        {
            decimal costo = 5000m; // Default
            try
            {
                if (System.IO.File.Exists(CONFIG_FILE))
                {
                    var json = System.IO.File.ReadAllText(CONFIG_FILE);
                    var config = System.Text.Json.JsonSerializer.Deserialize<ConfiguracionUpdateDto>(json);
                    if (config != null)
                    {
                        costo = config.CostoEnvio;
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback to default on error
                Console.WriteLine($"Error reading config: {ex.Message}");
            }

            return Ok(new ConfiguracionResponseDto { CostoEnvio = costo });
        }

        // PUT: api/Configuracion/envio
        [HttpPut("envio")]
        public ActionResult UpdateCostoEnvio([FromBody] ConfiguracionUpdateDto dto)
        {
            if (dto.CostoEnvio < 0)
            {
                return BadRequest(new { message = "El costo de envío no puede ser negativo" });
            }

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(dto);
                System.IO.File.WriteAllText(CONFIG_FILE, json);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error guardando configuración", error = ex.Message });
            }

            return Ok(new { message = "Costo de envío actualizado correctamente", costoEnvio = dto.CostoEnvio });
        }
    }

    public class ConfiguracionUpdateDto
    {
        public decimal CostoEnvio { get; set; }
    }

    public class ConfiguracionResponseDto
    {
        public decimal CostoEnvio { get; set; }
    }
}
