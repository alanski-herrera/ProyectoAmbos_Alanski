using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Models;
using ProyectoAmbos_Alanski.DTOs;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes
                .OrderByDescending(c => c.FechaRegistro)
                .ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResponseDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Reservas)
                .Include(c => c.Ventas)
                .Where(c => c.IdCliente == id)
                .Select(c => new ClienteResponseDto
                {
                    IdCliente = c.IdCliente,
                    NombreCliente = c.NombreCliente,
                    Dni = c.Dni,
                    Direccion = c.Direccion,
                    Telefono = c.Telefono,
                    Email = c.Email,
                    FechaRegistro = c.FechaRegistro,
                    Notas = c.Notas,
                    CantidadReservas = c.Reservas.Count,
                    CantidadCompras = c.Ventas.Count
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return Ok(cliente);
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(ClienteCreateDto dto)
        {
            var cliente = new Cliente
            {
                NombreCliente = dto.NombreCliente,
                Dni = dto.Dni,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Notas = dto.Notas,
                FechaRegistro = DateTime.Now
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, cliente);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteUpdateDto dto)
        {
            if (id != dto.IdCliente)
            {
                return BadRequest(new { message = "ID no coincide" });
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            cliente.NombreCliente = dto.NombreCliente;
            cliente.Dni = dto.Dni;
            cliente.Direccion = dto.Direccion;
            cliente.Telefono = dto.Telefono;
            cliente.Email = dto.Email;
            cliente.Notas = dto.Notas;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Clientes/buscar?telefono=351xxx
        [HttpGet("buscar")]
        public async Task<ActionResult<Cliente>> BuscarPorTelefono([FromQuery] string telefono)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Telefono == telefono);

            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return Ok(cliente);
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.IdCliente == id);
        }
    }
}