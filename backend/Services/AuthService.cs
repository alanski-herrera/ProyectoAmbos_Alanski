using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.DTOs;
using ProyectoAmbos_Alanski.Models;

namespace ProyectoAmbos_Alanski.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> Login(LoginDto loginDto)
        {
            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Email == loginDto.Email && a.Activo);

            if (admin == null)
            {
                return null;
            }

            // ✅ VERIFICAR CONTRASEÑA CON BCRYPT
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Contrasena, admin.Contrasena))
            {
                return null;
            }

            // Actualizar último acceso
            admin.UltimoAcceso = DateTime.UtcNow; // ← Cambiar a UTC

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar último acceso: {ex.Message}");
                // No falla el login si no puede actualizar
            }

            var token = GenerarToken(admin);

            return new LoginResponseDto
            {
                IdAdmin = admin.IdAdmin,
                NombreAdmin = admin.NombreAdmin,
                Email = admin.Email,
                Token = token
            };
        }

        public string GenerarToken(Administrador admin)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "tu-super-clave-secreta-muy-segura-minimo-32-caracteres";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "AmbosAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "AmbosClients";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.IdAdmin.ToString()),
                new Claim(ClaimTypes.Name, admin.NombreAdmin),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashPassword(string password)
        {
            // Por ahora retorna la contraseña tal cual
            // En producción usa BCrypt.Net
            return password;
        }

        public bool VerificarPassword(string password, string hashedPassword)
        {
            // Por ahora compara directamente
            // En producción usa BCrypt.Net
            return password == hashedPassword;
        }

        public async Task<Administrador?> Register(RegisterDto registerDto)
        {
            // Verificar si el email ya existe
            var existingAdmin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Email == registerDto.Email);

            if (existingAdmin != null)
            {
                return null; // Email ya registrado
            }

            // Verificar si el DNI ya existe
            var existingDni = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Dni == registerDto.Dni);

            if (existingDni != null)
            {
                return null; // DNI ya registrado
            }

            // Hashear la contraseña
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Contrasena);

            // Crear nuevo administrador
            var nuevoAdmin = new Administrador
            {
                NombreAdmin = registerDto.NombreAdmin,
                Dni = registerDto.Dni,
                Email = registerDto.Email,
                Contrasena = hashedPassword,
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            };

            _context.Administradores.Add(nuevoAdmin);
            await _context.SaveChangesAsync();

            return nuevoAdmin;
        }
    }
}