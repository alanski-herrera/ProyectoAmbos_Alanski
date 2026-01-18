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

var usePostgres = connectionString?.Contains("postgres") ?? false;

if (usePostgres)
{
    // PostgreSQL para producción (Render)
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null
            )
        )
    );
    Console.WriteLine("🔵 Usando PostgreSQL");
}
else
{
    // MySQL para desarrollo local
    // Convertir formato Railway (mysql://user:pass@host:port/db) a formato MySQL estándar
    if (connectionString != null && connectionString.StartsWith("mysql://"))
    {
        var uri = new Uri(connectionString);
        connectionString = $"Server={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};User={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};";
    }

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
    Console.WriteLine("🟢 Usando MySQL");
}

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// En producción (Render) no usar HTTPS redirect porque Render maneja SSL
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngular");

// IMPORTANTE: Authentication debe ir antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

// ===== Configurar puerto dinámico =====
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var url = $"http://0.0.0.0:{port}";

Console.WriteLine("🚀 API de AMBOS iniciada correctamente");
Console.WriteLine($"📦 Base de datos conectada");
Console.WriteLine($"🌐 Escuchando en: {url}");

app.Run(url);