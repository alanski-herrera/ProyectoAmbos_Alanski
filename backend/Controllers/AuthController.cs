using Microsoft.AspNetCore.Mvc;
using ProyectoAmbos_Alanski.DTOs;
using ProyectoAmbos_Alanski.Services;

namespace ProyectoAmbos_Alanski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(result);
        }

        // POST: api/Auth/logout (opcional)
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // En JWT no hay logout del lado del servidor
            // El cliente debe eliminar el token
            return Ok(new { message = "Sesión cerrada correctamente" });
        }
    }
}
