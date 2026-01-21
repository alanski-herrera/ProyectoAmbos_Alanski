using ProyectoAmbos_Alanski.DTOs;
using ProyectoAmbos_Alanski.Models;

namespace ProyectoAmbos_Alanski.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> Login(LoginDto loginDto);
        Task<Administrador?> Register(RegisterDto registerDto);
        string GenerarToken(Administrador admin);
        string HashPassword(string password);
        bool VerificarPassword(string password, string hashedPassword);
    }
}