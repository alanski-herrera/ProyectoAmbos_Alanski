using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using ProyectoAmbos_Alanski.Data;
using ProyectoAmbos_Alanski.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuración de la Base de Datos =====
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Convertir formato Railway MySQL (mysql://user:pass@host:port/db) a formato estándar
if (connectionString != null && connectionString.StartsWith("mysql://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Server={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};User={userInfo[0]};Password={userInfo[1]};";
}

// Usar MySQL (Railway)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

Console.WriteLine("🟢 Usando MySQL en Railway");

// ===== Configuración de JWT =====
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? builder.Configuration["Jwt:Key"]
    ?? "tu-super-clave-secreta-muy-segura-minimo-32-caracteres";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AmbosAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "AmbosClients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ===== Registrar Servicios =====
builder.Services.AddScoped<IAuthService, AuthService>();

// ===== Configuración de CORS =====
var allowedOriginsString = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")
    ?? builder.Configuration["AllowedOrigins"]
    ?? "http://localhost:4200";
var allowedOrigins = allowedOriginsString.Split(",");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ===== Agregar Controllers =====
builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// ===== Configuración de Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Configuración del pipeline HTTP =====
// Swagger siempre disponible para facilitar testing
app.UseSwagger();
app.UseSwaggerUI();

// Railway maneja SSL, no usar HTTPS redirect en producción
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngular");

// IMPORTANTE: Authentication debe ir antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Endpoint raíz para verificar que la API funciona
app.MapGet("/", () => new
{
    message = "API de AMBOS funcionando correctamente 🚀",
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
    database = "MySQL (Railway)"
});

app.MapControllers();

// ===== Aplicar migraciones automáticamente al iniciar =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Aplica migraciones pendientes
        Console.WriteLine("✅ Migraciones de base de datos aplicadas correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al aplicar migraciones: {ex.Message}");
    }
}

// ===== Configurar puerto dinámico para Railway =====
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var url = $"http://0.0.0.0:{port}";

Console.WriteLine("🚀 API de AMBOS iniciada correctamente");
Console.WriteLine($"📦 Base de datos: MySQL conectada");
Console.WriteLine($"🌐 Escuchando en: {url}");

app.Run(url);